using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Group.Model;
using NexusForever.Game.Static.Group;
using NexusForever.Game.Static.Matching;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Server.GroupServer.Character;
using NexusForever.Server.GroupServer.Network.Internal;

namespace NexusForever.Server.GroupServer.Group
{
    public class Group : IWrappedModel<GroupModel>
    {
        public GroupModel Model { get; private set; }

        public ulong Id => Model.GroupId;

        public GroupFlags Flags
        {
            get => Model.Flags;
            set => Model.Flags = value;
        }

        public LootRule LootRule
        {
            get => Model.LootRule;
            set => Model.LootRule = value;
        }

        public LootRule LootRuleThreshold
        {
            get => Model.LootRuleThreshold;
            set => Model.LootRuleThreshold = value;
        }

        public LootThreshold LootThreshold
        {
            get => Model.LootThreshold;
            set => Model.LootThreshold = value;
        }

        public HarvestLootRule LootRuleHarvest
        {
            get => Model.LootRuleHarvest;
            set => Model.LootRuleHarvest = value;
        }

        public Guid? Match
        {
            get => Model.Match;
            set => Model.Match = value;
        }

        public MatchTeam? MatchTeam
        {
            get => Model.MatchTeam;
            set => Model.MatchTeam = value;
        }

        public Identity Leader
        {
            get => Model.Leader != null ? new Identity
            {
                Id      = Model.Leader.CharacterId,
                RealmId = Model.Leader.RealmId
            } : null;
            set
            {
                Model.Leader = new GroupLeaderModel
                {
                    CharacterId = value.Id,
                    RealmId     = value.RealmId
                };
            }
        }

        public GroupRequest Request
        {
            get => _request;
            private set
            {
                _request      = value;
                Model.Request = _request?.Model;
            }
        }

        private GroupRequest _request;

        /// <summary>
        /// Determines if this group is an open world group.
        /// </summary>
        public bool IsOpenWorld => Flags.HasFlag(GroupFlags.OpenWorld);

        /// <summary>
        /// Determines if this group is a raid group.
        /// </summary>
        public bool IsRaid => Flags.HasFlag(GroupFlags.Raid);

        private readonly Dictionary<Game.Static.Group.GroupMarker, GroupMarker> _markers = [];
        private readonly Dictionary<Identity, GroupMember> _members = [];
        private readonly Dictionary<Identity, GroupInvite> _invites = [];

        #region Dependency Injection

        private IInternalMessagePublisher _messagePublisher;
        private readonly GroupManager _groupManager;
        private CharacterManager _characterManager;
        private readonly IServiceProvider _serviceProvider;

        public Group(
            OutboxMessagePublisher messagePublisher,
            GroupManager groupManager,
            CharacterManager characterManager,
            IServiceProvider serviceProvider)
        {
            _messagePublisher = messagePublisher;
            _groupManager     = groupManager;
            _characterManager = characterManager;
            _serviceProvider  = serviceProvider;
        }

        #endregion

        /// <summary>
        /// Initialise a new group with default values.
        /// </summary>
        public void Initialise()
        {
            if (Model != null)
                throw new InvalidOperationException("Group already initialised.");

            Model             = new GroupModel();
            LootRule          = LootRule.NeedBeforeGreed;
            LootRuleThreshold = LootRule.NeedBeforeGreed;
            LootThreshold     = LootThreshold.Good;
            LootRuleHarvest   = HarvestLootRule.FirstTagger;
        }

        /// <summary>
        /// Initialise group with a database model.
        /// </summary>
        /// <param name="model">Database model to initialise group.</param>
        public void Initialise(GroupModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("Group already initialised.");

            Model = model;

            if (model.Request != null)
            {
                var request = new GroupRequest();
                request.Initialise(model.Request);
                Request = request;
            }

            foreach (GroupMarkerModel markerModel in model.Markers)
            {
                var marker = _serviceProvider.GetRequiredService<GroupMarker>();
                marker.Initialise(this, markerModel);
                _markers.Add(marker.Marker, marker);
            }

            foreach (GroupMemberModel item in model.Members)
            {
                var member = _serviceProvider.GetRequiredService<GroupMember>();
                member.Initialise(this, item);
                _members.Add(member.Identity, member);
            }

            foreach (GroupInviteModel inviteModel in model.Invites)
            {
                var invite = new GroupInvite();
                invite.Initialise(inviteModel);
                _invites.Add(invite.Invitee, invite);
            }
        }

        /// <summary>
        /// Get the maximum group size.
        /// </summary>
        public uint GetMaxGroupSize()
        {
            if (IsRaid)
                return 20;
            else
                return 5;
        }

        /// <summary>
        /// Get a specific member in the group.
        /// </summary>
        /// <param name="identity">Identity of the member to return.</param>
        public GroupMember GetMember(Identity identity)
        {
            if (_members.TryGetValue(identity, out GroupMember member))
                return member;

            return null;
        }

        /// <summary>
        /// Get all members in the group.
        /// </summary>
        public IEnumerable<GroupMember> GetMembers()
        {
            return _members.Values;
        }

        /// <summary>
        /// Add a character to the group.
        /// </summary>
        /// <param name="identity">Identity of the character to add to the group.</param>
        public async Task<GroupMember> AddMemberAsync(Identity identity)
        {
            Character.Character character = await _characterManager.GetCharacterRemoteAsync(identity);
            if (character == null)
                return null;

            return await AddMemberAsync(character);
        }

        /// <summary>
        /// Add a character to the group.
        /// </summary>
        /// <param name="character">Character to add to the group.</param>
        public async Task<GroupMember> AddMemberAsync(Character.Character character)
        {
            if (GetMember(character.Identity) != null)
                return null;

            Leader ??= character.Identity;

            var member = _serviceProvider.GetRequiredService<GroupMember>();
            member.Initialise(this);
            member.Identity = character.Identity;
            member.Index    = NextMemberId();
            member.Flags    = Leader == character.Identity ? GroupMemberInfoFlags.GroupAdminFlags : GroupMemberInfoFlags.GroupMemberFlags;
            _members.Add(member.Identity, member);

            Model.Members.Add(member.Model);

            await character.AddGroupAsync(this);
            
            await _messagePublisher.PublishAsync(new GroupMemberAddedMessage
            {
                Group       = await this.ToInternalGroup(),
                AddedMember = await member.ToInternalGroupMember()
            });

            return member;
        }

        private uint NextMemberId()
        {
            HashSet<uint> set = _members.Values
                .Select(m => m.Index)
                .ToHashSet();

            uint i = 1;
            while (set.Contains(i))
                i++;

            return i;
        }

        /// <summary>
        /// Remove a member from the group.
        /// </summary>
        /// <param name="removed">Identity of the member being removed.</param>
        /// <param name="removeReason">Reason for the member removal.</param>
        /// <returns></returns>
        public async Task<GroupActionResult> RemoveMemberAsync(Identity removed, RemoveReason removeReason)
        {
            GroupMember member = GetMember(removed);
            if (member == null)
                return GroupActionResult.InvalidGroup;

            await RemoveMemberAsync(member, removeReason);

            return GroupActionResult.LeaveSuccess;
        }

        private async Task RemoveMemberAsync(GroupMember member, RemoveReason removeReason)
        {
            if (_members.Count <= 2 && IsOpenWorld)
            {
                await DisbandAsync();
                return;
            }

            if (member.Identity == Leader)
            {
                GroupMember newLeader = _members.FirstOrDefault(m => m.Key != member.Identity).Value;
                if (newLeader == null)
                {
                    await DisbandAsync();
                    return;
                }
                
                await PromoteMemberAsync(newLeader);
            }

            Character.Character character = await _characterManager.GetCharacterRemoteAsync(member.Identity);
            await character?.RemoveGroupAsync(this, removeReason);

            await _messagePublisher.PublishAsync(new GroupMemberRemovedMessage
            {
                Group         = await this.ToInternalGroup(),
                RemovedMember = await member.ToInternalGroupMember(),
                Reason        = removeReason
            });

            _members.Remove(member.Identity);
            Model.Members.Remove(member.Model);
        }

        /// <summary>
        /// Kick a member from the group.
        /// </summary>
        /// <param name="kicker">Identity of the member kicking.</param>
        /// <param name="kicked">Identity of the member being kicked.</param>
        /// <param name="removeReason">Reason for the member kick.</param>
        public async Task<GroupActionResult> KickMemberAsync(Identity kicker, Identity kicked, RemoveReason removeReason)
        {
            GroupMember kickerMember = GetMember(kicker);
            if (kickerMember == null)
                return GroupActionResult.InvalidGroup;

            if (!kickerMember.Flags.HasFlag(GroupMemberInfoFlags.CanKick))
                return GroupActionResult.KickFailed;

            GroupMember kickedMember = GetMember(kicked);
            if (kickedMember == null)
                return GroupActionResult.InvalidGroup;

            await RemoveMemberAsync(kickedMember, removeReason);

            return GroupActionResult.KickSuccess;
        }

        /// <summary>
        /// Disband the group.
        /// </summary>
        /// <param name="disbander">Identity of the member disbanding the group.</param>
        public async Task<GroupActionResult> DisbandAsync(Identity disbander)
        {
            if (disbander != Leader)
                return GroupActionResult.DisbandFailed;

            await DisbandAsync();

            return GroupActionResult.DisbandSuccess;
        }

        /// <summary>
        /// Disband the group.
        /// </summary>
        public async Task DisbandAsync()
        {
            await _messagePublisher.PublishAsync(new GroupDisbandedMessage
            {
                Group = await this.ToInternalGroup()
            });

            foreach (GroupMember groupMember in _members.Values)
            {
                Character.Character character = await _characterManager.GetCharacterRemoteAsync(groupMember.Identity);
                await character?.RemoveGroupAsync(this, RemoveReason.Disband);
            }

            _groupManager.RemoveGroup(this);
        }

        private GroupInvite GetInvite(Identity identity)
        {
            if (_invites.TryGetValue(identity, out GroupInvite invite))
                return invite;

            return null;
        }

        /// <summary>
        /// Invite a character to the group.
        /// </summary>
        /// <param name="inviter">Identity of the member inviting the character.</param>
        /// <param name="invitee">Character to invite to the group.</param>
        public async Task<GroupInviteResult> InviteMemberAsync(Identity inviter, Character.Character invitee)
        {
            GroupMember groupMember = GetMember(inviter);
            if (groupMember == null)
                return GroupInviteResult.GroupNotFound;

            if (Leader != inviter && !groupMember.Flags.HasFlag(GroupMemberInfoFlags.CanInvite))
                return GroupInviteResult.NoPermissions;

            if (GetMember(invitee.Identity) != null)
                return GroupInviteResult.Grouped;

            if (GetInvite(invitee.Identity) != null)
                return GroupInviteResult.Pending;

            if (_members.Count >= GetMaxGroupSize())
                return GroupInviteResult.Full;

            var invite = new GroupInvite();
            invite.Initialise();
            invite.Inviter    = inviter;
            invite.Invitee    = invitee.Identity;
            invite.Expiration = DateTime.UtcNow.AddSeconds(30);

            _invites.Add(invitee.Identity, invite);
            Model.Invites.Add(invite.Model);

            await _messagePublisher.PublishAsync(new GroupPlayerInvitedMessage
            {
                Group           = await this.ToInternalGroup(),
                Leader          = await GetMember(Leader).ToInternalGroupMember(),
                Inviter         = await groupMember.ToInternalGroupMember(),
                InviteeIdentity = invitee.Identity.ToInternalIdentity(),
                Invitee         = invitee.ToGroupCharacter()
            });

            return GroupInviteResult.Sent;
        }

        /// <summary>
        /// Respond to a group invite.
        /// </summary>
        /// <param name="invitee">Identity of the character responding to a group invite.</param>
        /// <param name="response">Reponse to the group invite.</param>
        public async Task InviteMemberResponseAsync(Identity invitee, bool response)
        {
            GroupInvite invite = GetInvite(invitee);
            if (invite == null)
                return;

            if (response)
            {
                GroupInviteResult result = InviteMemberReponse(invite);
                if (result == GroupInviteResult.Accepted)
                    await AddMemberAsync(invite.Invitee);

                await InviteMemberResponseAsync(invite.Inviter, invite.Invitee, result);
            }
            else
                await InviteMemberResponseAsync(invite.Inviter, invite.Invitee, GroupInviteResult.Declined);

            _invites.Remove(invite.Invitee);
            Model.Invites.Remove(invite.Model);
        }

        private GroupInviteResult InviteMemberReponse(GroupInvite invite)
        {
            if (_members.Count >= GetMaxGroupSize())
                return GroupInviteResult.Full;

            return GroupInviteResult.Accepted;
        }

        private async Task InviteMemberResponseAsync(Identity recipient, Identity target, GroupInviteResult result)
        {
            Character.Character character = await _characterManager.GetCharacterRemoteAsync(target);
            if (character == null)
                return;

            await _messagePublisher.PublishAsync(new GroupPlayerInviteResultMessage
            {
                Recipient = recipient.ToInternalIdentity(),
                Target    = character.IdentityName.ToInternalIdentity(),
                Result    = result,
            });
        }

        /// <summary>
        /// Expire any pending group invites that have not been accepted within the time limit.
        /// </summary>
        public async Task InvitesExpireAsync()
        {
            foreach (GroupInvite invite in _invites.Values)
            {
                if (invite.Expiration > DateTime.UtcNow)
                    continue;

                await InviteMemberResponseAsync(invite.Inviter, invite.Invitee, GroupInviteResult.ExpiredInviter);
                await InviteMemberResponseAsync(invite.Invitee, invite.Inviter, GroupInviteResult.ExpiredInvitee);

                _invites.Remove(invite.Invitee);
                Model.Invites.Remove(invite.Model);
            }
        }

        /// <summary>
        /// Refer a character to the group.
        /// </summary>
        /// <param name="referer">Identity of the member referring the character.</param>
        /// <param name="referee">Character being referred to the group.</param>
        public async Task<GroupInviteResult> ReferMemberAsync(Identity referer, Character.Character referee)
        {
            if (Flags.HasFlag(GroupFlags.ReferralsClosed))
                return GroupInviteResult.NotAcceptingRequests;

            GroupInviteResult? result = await RequestMemberAsync(referer, referee, GroupRequestType.Referral);
            return result ?? GroupInviteResult.SentToLeader;
        }

        /// <summary>
        /// Request to join the group.
        /// </summary>
        /// <param name="requester">Character requesting to join the group.</param>
        public async Task<GroupInviteResult> RequestMemberAsync(Character.Character requester)
        {
            if (Flags.HasFlag(GroupFlags.JoinRequestClosed))
                return GroupInviteResult.NotAcceptingRequests;

            GroupInviteResult? result = await RequestMemberAsync(requester.Identity, requester, GroupRequestType.Request);
            return result ?? GroupInviteResult.Sent;
        }

        private async Task<GroupInviteResult?> RequestMemberAsync(Identity requester, Character.Character requestee, GroupRequestType type)
        {
            if (GetMember(requestee.Identity) != null)
                return GroupInviteResult.Grouped;

            if (_request != null)
            {
                if (_request.RequesterIdentity == requestee.Identity)
                    return GroupInviteResult.InvitedYou;
                else
                    return GroupInviteResult.Busy;
            }

            if (_members.Count >= GetMaxGroupSize())
                return GroupInviteResult.Full;

            GroupMember leader = GetMember(Leader);
            if (leader == null || leader.Flags.HasFlag(GroupMemberInfoFlags.Disconnected))
                return GroupInviteResult.LeaderOffline;

            var request = new GroupRequest();
            request.Initialise();
            request.RequesterIdentity = requester;
            request.RequesteeIdentity = requestee.Identity;
            request.Type              = type;
            request.Expiration        = DateTime.UtcNow.AddSeconds(30);
            Request = request;

            await _messagePublisher.PublishAsync(new GroupMemberRequestedMessage
            {
                Group             = await this.ToInternalGroup(),
                RequesterIdentity = requestee.Identity.ToInternalIdentity(),
                Requester         = requestee.ToGroupCharacter(),
            });

            return null;
        }

        /// <summary>
        /// Respond to pending group request.
        /// </summary>
        /// <param name="respondent">Identity of the group member responding to the group request.</param>
        /// <param name="response">Reponse to the pending group request.</param>
        public async Task RequestMemberResponseAsync(Identity respondent, bool response)
        {
            if (_request == null)
                return;

            if (Leader != respondent)
                return;

            if (response)
            {
                await AddMemberAsync(_request.RequesteeIdentity);

                if (_request.Type == GroupRequestType.Request)
                    await RequestMemberResponseAsync(_request.RequesterIdentity, respondent, GroupInviteResult.Accepted);
                else
                    await RequestMemberResponseAsync(_request.RequesterIdentity, _request.RequesteeIdentity, GroupInviteResult.Accepted);
            }
            else
            {
                if (_request.Type == GroupRequestType.Request)
                    await RequestMemberResponseAsync(_request.RequesterIdentity, respondent, GroupInviteResult.Declined);
                else
                    await RequestMemberResponseAsync(_request.RequesterIdentity, _request.RequesteeIdentity, GroupInviteResult.Declined);
            }

            Request = null;
        }

        private async Task RequestMemberResponseAsync(Identity recipient, Identity target, GroupInviteResult result)
        {
            Character.Character character = await _characterManager.GetCharacterRemoteAsync(target);
            if (character == null)
                return;

            await _messagePublisher.PublishAsync(new GroupMemberRequestResultMessage
            {
                Recipient = recipient.ToInternalIdentity(),
                Target    = character.IdentityName.ToInternalIdentity(),
                Result    = result,
                Type      = _request.Type
            });
        }

        /// <summary>
        /// Expire pending group request if it hasn't responded to within the time limit.
        /// </summary>
        public async Task RequestMemberExpireAsync()
        {
            if (_request == null)
                return;

            if (_request.Expiration > DateTime.UtcNow)
                return;

            await RequestMemberResponseAsync(_request.RequesterIdentity, _request.RequesteeIdentity, GroupInviteResult.ExpiredInviter);
            await RequestMemberResponseAsync(Leader, _request.RequesteeIdentity, GroupInviteResult.ExpiredInvitee);

            Request = null;
        }

        /// <summary>
        /// Promote a member to group leader.
        /// </summary>
        /// <param name="promoter">Identity of the member promoting.</param>
        /// <param name="promotee">Identity of the member to be promoted.</param>
        public async Task<GroupActionResult> PromoteMemberAsync(Identity promoter, Identity promotee)
        {
            if (promoter != Leader)
                return GroupActionResult.PromoteFailed;

            GroupMember promoteeMember = GetMember(promotee);
            if (promoteeMember == null)
                return GroupActionResult.InvalidGroup;

            await PromoteMemberAsync(promoteeMember);

            return GroupActionResult.PromoteSuccess;
        }

        private async Task PromoteMemberAsync(GroupMember promotee)
        {
            GroupMember leaderMember = GetMember(Leader);
            await leaderMember.RemoveFlagAsync(GroupMemberInfoFlags.GroupAdminFlags, fromPromotion: true);

            Leader = promotee.Identity;

            await promotee.SetFlagAsync(GroupMemberInfoFlags.GroupAdminFlags, fromPromotion: true);

            await _messagePublisher.PublishAsync(new GroupMemberPromotedMessage
            {
                Group  = await this.ToInternalGroup(),
                Member = await promotee.ToInternalGroupMember(),
            });
        }

        /// <summary>
        /// Sets the member flags for a group member.
        /// </summary>
        /// <param name="updater">Identity of the member update the group member flags.</param>
        /// <param name="target">Identity of the member having member flags set.</param>
        /// <param name="flags">Flags to set or unset.</param>
        public async Task<GroupActionResult> SetMemberFlagsAsync(Identity updater, Identity target, GroupMemberInfoFlags flags)
        {
            GroupMember member = GetMember(target);
            if (member == null)
                return GroupActionResult.InvalidGroup;

            if (updater != Leader
                && !member.CanSetFlags(updater, flags))
                return GroupActionResult.MemberFlagsFailed;

            if ((member.Flags & flags) == 0)
                await member.SetFlagAsync(flags);
            else
                await member.RemoveFlagAsync(flags);

            return GroupActionResult.MemberFlagsSuccess;
        }

        /// <summary>
        /// Sets the group flags for the group.
        /// </summary>
        /// <param name="updater">Identity of the member updating the group flags.</param>
        /// <param name="groupFlags">Set group flags to set.</param>
        public async Task<GroupActionResult> SetGroupFlagsAsync(Identity updater, GroupFlags groupFlags)
        {
            if (updater != Leader)
                return GroupActionResult.FlagsFailed;

            bool setToRaid = !IsRaid && groupFlags.HasFlag(GroupFlags.Raid);
            Flags = groupFlags;

            await _messagePublisher.PublishAsync(new GroupFlagsUpdatedMessage
            {
                Group = await this.ToInternalGroup(),
            });

            if (setToRaid)
            {
                await _messagePublisher.PublishAsync(new GroupMaxSizeUpdatedMessage
                {
                    Group = await this.ToInternalGroup(),
                });
            }

            return GroupActionResult.FlagsSuccess;
        }

        /// <summary>
        /// Sets the loot rules for the group.
        /// </summary>
        /// <param name="updater">The identity of the member updating the loot rules.</param>
        /// <param name="normalRule"></param>
        /// <param name="thresholdRule"></param>
        /// <param name="thresholdQuality"></param>
        /// <param name="harvestRule"></param>
        public async Task<GroupActionResult> SetLootRulesAsync(Identity updater, LootRule normalRule, LootRule thresholdRule, LootThreshold thresholdQuality, HarvestLootRule harvestRule)
        {
            var member = GetMember(updater);
            if (member == null)
                return GroupActionResult.InvalidGroup;

            if (member.Identity != Leader)
                return GroupActionResult.ChangeSettingsFailed;

            LootRule          = normalRule;
            LootRuleThreshold = thresholdRule;
            LootThreshold     = thresholdQuality;
            LootRuleHarvest   = harvestRule;

            await _messagePublisher.PublishAsync(new GroupLootRulesUpdatedMessage
            {
                Group = await this.ToInternalGroup()
            });

            return GroupActionResult.ChangeSettingsSuccess;
        }

        /// <summary>
        /// Start a ready check for the group.
        /// </summary>
        /// <param name="initiator"></param>
        /// <param name="message"></param>
        public async Task<GroupActionResult?> StartReadyCheckAsync(Identity initiator, string message)
        {
            GroupMember initiatorMember = GetMember(initiator);
            if (initiatorMember == null)
                return GroupActionResult.InvalidGroup;

            if (initiator != Leader)
            {
                if (!IsRaid)
                    return GroupActionResult.ReadyCheckFailed;
                else if (!initiatorMember.Flags.HasFlag(GroupMemberInfoFlags.CanReadyCheck))
                    return GroupActionResult.ReadyCheckFailed;
            }

            foreach (GroupMember member in _members.Values)
            {
                // deliberatley not using RemoveFlagAsync here to avoid sending 2 messages
                member.Flags &= ~(GroupMemberInfoFlags.HasSetReady | GroupMemberInfoFlags.Ready);
                await member.SetFlagAsync(GroupMemberInfoFlags.Pending);
            }

            await _messagePublisher.PublishAsync(new GroupReadyCheckStartedMessage
            {
                Group   = await this.ToInternalGroup(),
                Member  = await initiatorMember.ToInternalGroupMember(),
                Message = message,
            });

            // seems there is no success result for this action
            return null;
        }

        /// <summary>
        /// Get the target markers for the group.
        /// </summary>
        public IEnumerable<GroupMarker> GetMarkers()
        {
            return _markers.Values;
        }

        private GroupMarker GetMarker(Game.Static.Group.GroupMarker marker)
        {
            if (_markers.TryGetValue(marker, out GroupMarker groupMarker))
                return groupMarker;
            return null;
        }

        private GroupMarker GetMarker(uint unitId)
        {
            return _markers.Values.SingleOrDefault(m => m.Model.UnitId == unitId);
        }

        /// <summary>
        /// Set a target marker for a unit.
        /// </summary>
        /// <param name="source">Identity of the member setting the target market.</param>
        /// <param name="marker">Marker type to set.</param>
        /// <param name="unitId">Unit id of the entity to mark.</param>
        public async Task<GroupActionResult?> SetTargetMarkerAsync(Identity source, Game.Static.Group.GroupMarker marker, uint? unitId)
        {
            GroupMember sourceMember = GetMember(source);
            if (sourceMember == null)
                return GroupActionResult.MarkingNotPermitted;

            if (!sourceMember.Flags.HasFlag(GroupMemberInfoFlags.CanMark))
                return GroupActionResult.MarkingNotPermitted;

            if (marker is Game.Static.Group.GroupMarker.None or > Game.Static.Group.GroupMarker.UFO)
                return GroupActionResult.InvalidMarkTarget;

            if (!unitId.HasValue)
            {
                GroupMarker typeMarker = GetMarker(marker);
                if (typeMarker == null)
                    return GroupActionResult.InvalidMarkTarget;

                await RemoveTargetMarkerAsync(typeMarker);
            }
            else
            {
                GroupMarker typeMarker = GetMarker(marker);
                typeMarker ??= CreateTargetMarker(marker);

                GroupMarker unitMarker = GetMarker(unitId.Value);
                if (unitMarker != null && unitMarker != typeMarker)
                    await RemoveTargetMarkerAsync(unitMarker);

                await typeMarker.SetMarkerAsync(unitId);
            }

            // seems there is no success result for this action
            return null;
        }

        private GroupMarker CreateTargetMarker(Game.Static.Group.GroupMarker marker)
        {
            var typeMarker = _serviceProvider.GetRequiredService<GroupMarker>();
            typeMarker.Initialise(this);
            typeMarker.Marker = marker;
            _markers.Add(marker, typeMarker);
            Model.Markers.Add(typeMarker.Model);

            return typeMarker;
        }

        private async Task RemoveTargetMarkerAsync(GroupMarker marker)
        {
            // we want to set the marker to null before remove it to ensure the event is published
            await marker.SetMarkerAsync(null);
            _markers.Remove(marker.Marker);
            Model.Markers.Remove(marker.Model);
        }
    }
}
