using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Group;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.Session;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Shared.Game;
using NetworkGroupMember = NexusForever.Network.World.Message.Model.Shared.GroupMember;

namespace NexusForever.Game.Group
{
    public class Group : IGroup
    {
        private readonly Dictionary<ulong, IGroupInvite> invites = new Dictionary<ulong, IGroupInvite>();

        private Dictionary<ulong, IGroupMember> membershipsByCharacterID = new Dictionary<ulong, IGroupMember>();
        private List<IGroupMember> members = new List<IGroupMember>();

        public int MemberCount { get => members.Count; }

        private IGroupMarkerInfo markerInfo;

        private LootRule lootRule = LootRule.NeedBeforeGreed;
        private LootRule lootRuleThreshold = LootRule.RoundRobin;
        private HarvestLootRule lootRuleHarvest = HarvestLootRule.FirstTagger;
        private LootThreshold lootThreshold = LootThreshold.Good;

        /// <summary>
        /// Id for the current <see cref="Group"/>
        /// </summary>
        public ulong Id { get; }

        /// <summary>
        /// The <see cref="Group"/> leader
        /// </summary>
        public IGroupMember Leader { get; set; }

        /// <summary>
        /// <see cref="GroupFlags"/> for <see cref="Group"/>
        /// </summary>
        public GroupFlags Flags { get; set; }

        /// <summary>
        /// Max group size for <see cref="Group"/>
        /// </summary>
        public uint MaxGroupSize { get; set; }

        /// <summary>
        /// If this group is an open world group.
        /// </summary>
        public bool IsOpenWorld { get => Flags.HasFlag(GroupFlags.OpenWorld); }

        /// <summary>
        /// If this group is a raid.
        /// </summary>
        public bool IsRaid { get => Flags.HasFlag(GroupFlags.Raid); }

        /// <summary>
        /// True if the Group is full.
        /// </summary>
        public bool IsFull { get => members.Count >= MaxGroupSize; }

        private bool isNewGroup { get; set; }

        private UpdateTimer positionUpdateTimer;

        /// <summary>
        /// Creates an instance of <see cref="Group"/>
        /// </summary>
        protected Group(ulong id)
        {
            isNewGroup = true;
            Id = id;
            markerInfo = new GroupMarkerInfo(this);

            SetGroupSize();
            positionUpdateTimer = new UpdateTimer(1, false);
        }

        /// <summary>
        /// Invite the targeted <see cref="Player"/>
        /// </summary>
        public void Invite(IPlayer inviter, IPlayer invitedPlayer)
        {
            SendGroupResult(inviter.Session, GroupResult.Sent, Id, invitedPlayer.Name);

            IGroupInvite invite = CreateInvite(GetMembershipForGroupFromPlayer(inviter), invitedPlayer, GroupInviteType.Invite);
            SendInvite(invite);
        }

        /// <summary>
        /// Creates a new <see cref="GroupMember"/> for the target player is this group and assigned the membership to the player.
        /// to the <see cref="Group"/>
        /// </summary>
        public IGroupMember CreateMember(IPlayer player)
        {
            IGroupMember member = new GroupMember(NextMemberId(), this, player);
            members.Add(member);
            membershipsByCharacterID.Add(player.CharacterId, member);
            player.AddToGroup(member);
            return member;
        }

        public void Update(double lastTick)
        {
            foreach (GroupInvite invite in invites.Values)
            {
                invite.ExpirationTime -= lastTick;
                if (invite.ExpirationTime <= 0d)
                {
                    // Delete the current invite
                    ExpireInvite(invite);
                }
            }


            positionUpdateTimer.Update(lastTick);
            if (positionUpdateTimer.HasElapsed)
            {
                positionUpdateTimer.Reset();
                UpdatePositions();
            }
        }

        /// <summary>
        /// Get the next available MemberId
        /// </summary>
        public ulong NextMemberId()
        {
            if (members.Count > 0)
                return members.Last().Id + 1UL;
            else
                return 1;
        }

        /// <summary>
        /// Builds all <see cref="GroupMember"/>s into <see cref="NetworkGroupMember"/>s.
        /// </summary>
        public List<NetworkGroupMember> BuildGroupMembers()
        {
            List<NetworkGroupMember> memberList = new List<NetworkGroupMember>();
            foreach (var member in members)
            {
                NetworkGroupMember groupMember = member.Build();
                groupMember.GroupMemberId = (ushort)member.Id;
                memberList.Add(member.Build());
            }

            return memberList;
        }

        /// <summary>
        /// Builds all <see cref="GroupMember"/> into <see cref="GroupMemberInfo"/>
        /// </summary>
        public List<GroupMemberInfo> BuildMembersInfo()
        {
            List<GroupMemberInfo> memberList = new List<GroupMemberInfo>();

            foreach (var member in members)
                memberList.Add(member.BuildMemberInfo());

            return memberList;
        }

        /// <summary>
        /// Broadcast <see cref="IWritable"/> to all <see cref="GroupMember"/>
        /// in the <see cref="Group"/>
        /// </summary>
        public void BroadcastPacket(IWritable message)
        {
            foreach (var member in members)
            {
                // If the player is not online - can't give them the message.
                IPlayer player = PlayerManager.Instance.GetPlayer(member.CharacterId);
                player?.Session.EnqueueMessage(message);
            }
        }

        #region Invites

        /// <summary>
        /// Create a new <see cref="GroupInvite"/>
        /// </summary>
        public IGroupInvite CreateInvite(IGroupMember inviter, IPlayer invitedPlayer, GroupInviteType type)
        {
            IGroupInvite invite = new GroupInvite(NextInviteId(), this, invitedPlayer.CharacterId, invitedPlayer.Name, inviter, type);
            if (!invites.TryAdd(invite.InviteId, invite))
                return null;

            invitedPlayer.GroupInvite = invite;
            return invite;
        }

        /// <summary>
        /// Get the next available InviteId
        /// </summary>
        /// <returns></returns>
        public ulong NextInviteId()
        {
            if (invites.Count > 0)
                return invites.Last().Key + 1UL;
            else
                return 1;
        }

        /// <summary>
        /// Exprires the <see cref="GroupInvite"/> notifying all relevant parties of the expiration
        /// </summary>
        /// <param name="invite">The Invite to Expire.</param>
        public void ExpireInvite(IGroupInvite invite)
        {
            if (!invites.ContainsKey(invite.InviteId))
                return;

            RemoveInvite(invite);
            switch (invite.Type)
            {
                case GroupInviteType.Invite:
                    {
                        IPlayer leader = PlayerManager.Instance.GetPlayer(Leader.CharacterId);
                        if (leader != null)
                            SendGroupResult(leader.Session, GroupResult.ExpiredInviter, Id, invite.InvitedCharacterName);

                        IPlayer invited = PlayerManager.Instance.GetPlayer(Leader.CharacterId);
                        if (invited != null)
                            SendGroupResult(invited.Session, GroupResult.ExpiredInvitee, Id, invite.InvitedCharacterName);
                        break;
                    }
                case GroupInviteType.Request:
                case GroupInviteType.Referral:
                    break;
            }
        }

        /// <summary>
        /// Accepts the <see cref="GroupInvite"/> and Adds the Invited Player to the group.
        /// </summary>
        /// <param name="invite">The <see cref="GroupInvite"/> to accept.</param>
        public void AcceptInvite(string inviteeName)
        {
            IGroupInvite invite = invites.Values.SingleOrDefault(inv => inv.InvitedCharacterName.Equals(inviteeName));
            if (invite == null)
                return;

            AcceptInvite(invite);
        }
        /// <summary>
        /// Accepts the <see cref="GroupInvite"/> and Adds the Invited Player to the group.
        /// </summary>
        /// <param name="invite">The <see cref="GroupInvite"/> to accept.</param>
        public void AcceptInvite(IGroupInvite invite)
        {
            if (!invites.ContainsKey(invite.InviteId))
                return;

            // If the person who accepted the invite is not online - how TF did they accept the invite?
            IPlayer targetPlayer = PlayerManager.Instance.GetPlayer(invite.InvitedCharacterId);
            if (targetPlayer == null)
                return;

            IGroupMember addedMember = CreateMember(targetPlayer);
            if (addedMember == null)
                return;

            RemoveInvite(invite);
            AddMember(addedMember);

            IPlayer leader = PlayerManager.Instance.GetPlayer(Leader.CharacterId);

            switch (invite.Type)
            {
                case GroupInviteType.Invite:
                    if (leader != null)
                        SendGroupResult(leader.Session, GroupResult.Accepted, Id, targetPlayer.Name);

                    break;
                case GroupInviteType.Request:
                    targetPlayer.Session.EnqueueMessageEncrypted(new ServerGroupRequestJoinResult
                    {
                        GroupId = Id,
                        IsJoin = true,
                        Name = leader.Name,
                        Result = GroupResult.Accepted
                    });
                    break;
                case GroupInviteType.Referral:
                    break;
            }

        }

        /// <summary>
        /// Declines the <see cref="GroupInvite"/> by player name.
        /// </summary>
        public void DeclineInvite(string inviteeName)
        {
            IGroupInvite invite = invites.Values.SingleOrDefault(inv => inv.InvitedCharacterName.Equals(inviteeName));
            if (invite == null)
                return;

            DeclineInvite(invite);
        }
        /// <summary>
        /// Declines the <see cref="GroupInvite"/> notifying all relevant parties of the invite result.
        /// </summary>
        public void DeclineInvite(IGroupInvite invite)
        {
            if (!invites.ContainsKey(invite.InviteId))
                return;

            RemoveInvite(invite);

            IPlayer targetPlayer = PlayerManager.Instance.GetPlayer(invite.InvitedCharacterId); // presumable to decline the invite the player has to be online. Expire is handled seperatly.
            IPlayer leader = PlayerManager.Instance.GetPlayer(Leader.CharacterId);

            switch (invite.Type)
            {
                case GroupInviteType.Invite:
                    if (leader != null)
                        SendGroupResult(leader.Session, GroupResult.Declined, Id, invite.InvitedCharacterName);

                    break;

                case GroupInviteType.Request:
                    targetPlayer.Session.EnqueueMessageEncrypted(new ServerGroupRequestJoinResult
                    {
                        GroupId = Id,
                        IsJoin = false,
                        Name = leader.Name, //TODO: What if the Leader is offline?
                        Result = GroupResult.Declined
                    });
                    //GroupHandler.SendGroupResult(invite.TargetPlayer.Session, GroupResult.Declined, Id, Leader.Player.Name); //TODO: Does this need to be implemented?
                    break;

                case GroupInviteType.Referral:
                    //TODO: Does this need to be implemented?
                    break;
            }
        }

        /// <summary>
        /// Refers a <see cref="Player"/> to be invited to the guild.
        /// </summary>
        public void ReferMember(IGroupMember inviter, IPlayer invitee)
        {
            if (CreateInvite(inviter, invitee, GroupInviteType.Referral) == null)
                return;

            IPlayer leader = PlayerManager.Instance.GetPlayer(Leader.CharacterId);
            if (leader == null)
                return; //TODO: What if the leader is Offline? Who can we refer to? Should we refer to a raid assist or just noone and can we replay this invite once the leader comes back online?

            leader.Session.EnqueueMessageEncrypted(
                new ServerGroupReferral
                {
                    GroupId = Id,
                    InviteeIdentity = new TargetPlayerIdentity { CharacterId = invitee.CharacterId, RealmId = RealmContext.Instance.RealmId },
                    InviteeName = invitee.Name
                }
            );
        }

        private void RemoveInvite(IGroupInvite invite)
        {
            if (!invites.ContainsKey(invite.InviteId))
                return;

            // Remnove the pending invite so it doesnt get re-sent when somone comes online.
            invites.Remove(invite.InviteId);

            IPlayer targetPlayer = PlayerManager.Instance.GetPlayer(invite.InvitedCharacterId);
            if (targetPlayer == null)
                return;

            // If i cant get the player here, there is no player object to remove the invite from.
            targetPlayer.GroupInvite = null;
        }

        private void SendInvite(IGroupInvite invite)
        {
            IPlayer targetPlayer = PlayerManager.Instance.GetPlayer(invite.InvitedCharacterId);

            if (targetPlayer == null)
                return;

            targetPlayer.Session.EnqueueMessageEncrypted(new ServerGroupInviteReceived
            {
                GroupId = Id,
                InviterIndex = invite.Inviter.GroupIndex,
                LeaderIndex = Leader.GroupIndex,
                Members = BuildGroupMembers()
            });
        }
        #endregion

        /// <summary>
        /// Check if a <see cref="GroupMember"/> can join the <see cref="Group"/>
        /// </summary>
        public bool CanJoinGroup(out GroupResult result)
        {
            // Member count is over the max group member count.
            if (members.Count >= MaxGroupSize)
            {
                result = GroupResult.Full;
                return false;
            }

            // Join requests are closed.
            if ((Flags & GroupFlags.JoinRequestClosed) != 0)
            {
                result = GroupResult.NotAcceptingRequests;
                return false;
            }

            result = GroupResult.Sent;
            return true;
        }

        /// <summary>
        /// Add a new <see cref="GroupMember"/> to the <see cref="Group"/>
        /// </summary>
        public void AddMember(IGroupMember addedMember)
        {
            if (isNewGroup)
            {
                isNewGroup = false;
                positionUpdateTimer.Reset();

                foreach (var member in members)
                {
                    IPlayer player = PlayerManager.Instance.GetPlayer(member.CharacterId);
                    if (player == null)
                        continue;

                    ServerGroupJoin groupJoinPacket = new ServerGroupJoin
                    {
                        TargetPlayer = new TargetPlayerIdentity
                        {
                            CharacterId = player.CharacterId,
                            RealmId = RealmContext.Instance.RealmId
                        },
                        GroupInfo = Build()
                    };

                    player.Session.EnqueueMessageEncrypted(groupJoinPacket);
                    foreach (GroupMember member2 in members)
                        player.Session.EnqueueMessageEncrypted(member2.BuildGroupStatUpdate());
                }
            }
            else
            {
                IPlayer addedPlayer = PlayerManager.Instance.GetPlayer(addedMember.CharacterId);
                if (addedPlayer == null)
                    return;

                addedPlayer.Session.EnqueueMessageEncrypted(new ServerGroupJoin
                {
                    TargetPlayer = new TargetPlayerIdentity
                    {
                        CharacterId = addedPlayer.CharacterId,
                        RealmId = RealmContext.Instance.RealmId
                    },
                    GroupInfo = Build()
                });

                BroadcastPacket(new ServerGroupMemberAdd
                {
                    GroupId = Id,
                    AddedMemberInfo = addedMember.BuildMemberInfo()
                });
                foreach (IGroupMember member2 in members)
                    addedPlayer.Session.EnqueueMessageEncrypted(member2.BuildGroupStatUpdate());
            }
        }

        /// <summary>
        /// Kick a <see cref="GroupMember"/> from the <see cref="Group"/>.
        /// </summary>
        public void KickMember(TargetPlayerIdentity target)
        {
            // TODO: If WoW is anything to go by; instance groups do NOT disband like this; once the instance is closed the group will be cleaned up.// // TODO: If WoW is anything to go by; instance groups do NOT disband like this; once the instance is closed the group will be cleaned up.
            if (members.Count == 2 && IsOpenWorld)
            {
                Disband();
                return;
            }

            IGroupMember kickedMember = FindMember(target);
            if (kickedMember == null)
                return;

            if (kickedMember.IsPartyLeader)
                return;

            IPlayer kickedPlayer = PlayerManager.Instance.GetPlayer(kickedMember.CharacterId);
            members.Remove(kickedMember);
            membershipsByCharacterID.Remove(kickedMember.CharacterId);


            // Tell the player they are no longer in a group.
            if (kickedPlayer != null)
            {
                kickedPlayer.RemoveFromGroup(kickedMember);
                kickedPlayer.Session.EnqueueMessageEncrypted(new ServerGroupLeave
                {
                    GroupId = Id,
                    Reason = RemoveReason.Kicked
                });
            }

            // Tell Other memebers of the group this player has been kicked.
            BroadcastPacket(new ServerGroupRemove
            {
                GroupId = Id,
                Reason = RemoveReason.Kicked,
                TargetPlayer = target
            });
        }

        /// <summary>
        /// Removes the <see cref="GroupMember"/> from the group.
        /// </summary>
        /// <param name="memberToRemove"></param>
        public void RemoveMember(IGroupMember memberToRemove)
        {
            if (!this.members.Contains(memberToRemove))
                return;

            IPlayer removedPlayer = PlayerManager.Instance.GetPlayer(memberToRemove.CharacterId);
            members.Remove(memberToRemove);
            membershipsByCharacterID.Remove(memberToRemove.CharacterId);

            if (removedPlayer != null)
            {
                removedPlayer.RemoveFromGroup(memberToRemove);
                removedPlayer.Session.EnqueueMessageEncrypted(new ServerGroupLeave
                {
                    GroupId = Id,
                    Reason = RemoveReason.Left
                });
            }

            BroadcastPacket(new ServerGroupRemove
            {
                GroupId = Id,
                Reason = RemoveReason.Left,
                TargetPlayer = new TargetPlayerIdentity()
                {
                    CharacterId = memberToRemove.CharacterId,
                    RealmId = RealmContext.Instance.RealmId
                }
            });
        }

        /// <summary>
        /// Disbands and removes the group from the <see cref="GroupManager"/>
        /// </summary>
        public void Disband()
        {
            foreach (IGroupMember member in members)
            {
                IPlayer player = PlayerManager.Instance.GetPlayer(member.CharacterId);
                if (player == null)
                    continue;

                player.RemoveFromGroup(member);
            }

            BroadcastPacket(new ServerGroupLeave
            {
                GroupId = Id,
                Reason = RemoveReason.Disband
            });
            GroupManager.Instance.RemoveGroup(this);
        }

        /// <summary>
        /// Sets the <see cref="GroupFlags"/> on the group and broadcasts the changes to all members.
        /// </summary>
        /// <param name="newFlags"></param>
        public void SetGroupFlags(GroupFlags newFlags)
        {
            bool shouldSetToRaid = !IsRaid && newFlags.HasFlag(GroupFlags.Raid);
            Flags = newFlags;

            if (shouldSetToRaid)
                ConvertToRaid();

            BroadcastPacket(new ServerGroupFlagsChanged
            {
                GroupId = Id,
                Flags = Flags,
            });
        }

        /// <summary>
        /// Converts the Party to a raid
        /// </summary>
        public void ConvertToRaid()
        {
            SetGroupSize();
            BroadcastPacket(new ServerGroupMaxSizeChange
            {
                GroupId = Id,
                NewFlags = Flags,
                NewMaxSize = MaxGroupSize
            });
        }

        /// <summary>
        /// Prepares the group for a readycheck
        /// </summary>
        public void PrepareForReadyCheck()
        {
            foreach (IGroupMember member in members)
            {
                member.PrepareForReadyCheck();
                BroadcastPacket(new ServerGroupMemberFlagsChanged
                {
                    GroupId = Id,
                    ChangedFlags = member.Flags,
                    IsFromPromotion = false,
                    MemberIndex = member.GroupIndex,
                    TargetedPlayer = new TargetPlayerIdentity() { CharacterId = member.CharacterId, RealmId = RealmContext.Instance.RealmId },
                });
            }
        }

        /// <summary>
        /// Prepares the group for a readycheck
        /// </summary>
        public void PerformReadyCheck(IPlayer invoker, string message)
        {
            BroadcastPacket(new ServerGroupSendReadyCheck
            {
                GroupId = Id,
                Invoker = new TargetPlayerIdentity() { CharacterId = invoker.CharacterId, RealmId = RealmContext.Instance.RealmId },
                Message = message,
            });
        }

        /// <summary>
        /// Updates the Targeted Player Role.
        /// </summary>
        /// <param name="updater">The <see cref="GroupMember"/> attempting to update the Role of the target.</param>
        /// <param name="target">The Player whose <see cref="GroupMemberInfo"/> should be updated.</param>
        /// <param name="changedFlag">The flag to change</param>
        /// <param name="addPermission">If true, adds the permission to the <see cref="GroupMember"/> otherwise revokes it.</param>
        public void UpdateMemberRole(IGroupMember updater, TargetPlayerIdentity target, GroupMemberInfoFlags changedFlag, bool addPermission)
        {
            IGroupMember member = FindMember(target);
            if (member == null)
                return;

            if (!updater.CanUpdateFlags(changedFlag, member))
                return;

            foreach (IGroupMember groupMember in members)
            {
                if (groupMember.CharacterId == target.CharacterId)
                    break;
            }

            member.SetFlags(changedFlag, addPermission);
            BroadcastPacket(new ServerGroupMemberFlagsChanged
            {
                GroupId = Id,
                ChangedFlags = member.Flags,
                IsFromPromotion = false,
                MemberIndex = member.GroupIndex,
                TargetedPlayer = target
            });
        }

        /// <summary>
        ///
        /// </summary>
        public void HandleJoinRequest(IPlayer prospective)
        {
            if (CreateInvite(Leader, prospective, GroupInviteType.Request) == null)
                return;

            /** Currently assuming that most of this packet is feking useless - but its what is expected.
             * It seems stupid to send GroupMemeberInfo about somone who is not in the group.
             * If they are not in the group, GroupIndex and Flags are useless.
             */
            IPlayer leader = PlayerManager.Instance.GetPlayer(Leader.CharacterId);
            if (leader == null)
                return; //TODO: What if the Leader is offline? presumable nothing, invites should be resent when the leader logs back in?

            leader.Session.EnqueueMessageEncrypted(
                new ServerGroupRequestJoinResponse
                {
                    GroupId = Id,
                    MemberInfo = new GroupMemberInfo
                    {
                        Member = prospective.BuildGroupMember(),
                        Flags = 0,  // I am assuming this is useless, the client seems todo nothing with it
                        GroupIndex = 0, // I am assuming this is useless, the client seems todo nothing with it
                        MemberIdentity = new TargetPlayerIdentity() { CharacterId = prospective.CharacterId, RealmId = RealmContext.Instance.RealmId }
                    }
                }
            );
        }

        /// <summary>
        /// Updates the Groups Loot Rules.
        /// </summary>
        public void UpdateLootRules(LootRule lootRulesUnderThreshold, LootRule lootRulesThresholdAndOver, LootThreshold lootThreshold, HarvestLootRule harvestLootRule)
        {
            lootRule = lootRulesUnderThreshold;
            lootRuleThreshold = lootRulesThresholdAndOver;
            this.lootThreshold = lootThreshold;
            lootRuleHarvest = harvestLootRule;

            BroadcastPacket(new ServerGroupLootRulesChange
            {
                GroupId = Id,
                UnknownDWord = 0, // maybe characterId?
                LootRulesUnderThreshold = lootRulesUnderThreshold,
                LootRulesThresholdAndOver = lootRulesThresholdAndOver,
                LootThreshold = lootThreshold,
                HarvestLootRule = harvestLootRule,
            });
        }

        /// <summary>
        /// Marks the specified unitId with the <see cref="GroupMarker"/>.
        /// </summary>
        public void MarkUnit(uint unitId, GroupMarker marker)
        {
            markerInfo.MarkTarget(unitId, marker);
        }

        /// <summary>
        /// Promotes a <see cref="GroupMember"/> to be the new leader of the group.
        /// </summary>
        /// <param name="newLeader"></param>
        public void Promote(TargetPlayerIdentity newLeader)
        {
            IGroupMember memberToPromote = Leader;
            foreach (IGroupMember member in members)
            {
                memberToPromote = member;
                if (member.CharacterId == newLeader.CharacterId)
                    break;
            }
            Leader = memberToPromote;

            BroadcastPacket(new ServerGroupPromote
            {
                GroupId = Id,
                LeaderIndex = Leader.GroupIndex,
                NewLeader = newLeader
            });
        }

        /// <summary>
        /// Find a <see cref="GroupMember"/> with the provided <see cref="TargetPlayerIdentity"/>
        /// </summary>
        public IGroupMember FindMember(TargetPlayerIdentity target)
        {
            if (!membershipsByCharacterID.ContainsKey(target.CharacterId))
                return null;

            return membershipsByCharacterID[target.CharacterId];
        }

        /// <summary>
        /// Gets the Index of The target <see cref="GroupMember"/>.
        /// </summary>
        public uint GetMemberIndex(IGroupMember groupMember)
        {
            if (groupMember.Group.Id != this.Id)
                throw new InvalidOperationException("Member does not belong to this group.");

            return (uint)this.members.IndexOf(groupMember);
        }

        private void SetGroupSize()
        {
            if (IsRaid)
                MaxGroupSize = 20;
            else
                MaxGroupSize = 5;
        }

        /// <summary>
        /// Build the <see cref="GroupInfo"/> structure from the current <see cref="Group"/>
        /// </summary>
        public GroupInfo Build()
        {
            return new GroupInfo
            {
                GroupId = Id,
                Flags = Flags,
                LeaderIdentity = new TargetPlayerIdentity
                {
                    CharacterId = Leader.CharacterId,
                    RealmId = RealmContext.Instance.RealmId
                },
                LootRule = lootRule,
                LootRuleThreshold = lootRuleThreshold,
                LootRuleHarvest = lootRuleHarvest,
                LootThreshold = lootThreshold,
                MaxGroupSize = MaxGroupSize,
                MemberInfos = BuildMembersInfo(),
                RealmId = RealmContext.Instance.RealmId,
                MarkerInfo = markerInfo.Build()
            };
        }

        private IGroupMember GetMembershipForGroupFromPlayer(IPlayer player)
        {
            if (player.GroupMembership1.Group.Id == Id)
                return player.GroupMembership1;

            if (player.GroupMembership2.Group.Id == Id)
                return player.GroupMembership2;

            throw new InvalidOperationException("Player is not a member of this group.");
        }

        private void UpdatePositions()
        {
            foreach (var member in members)
            {
                IPlayer player = PlayerManager.Instance.GetPlayer(member.CharacterId);
                if (player == null)
                    continue;

                if (member.ZoneId != player.Zone.Id)
                {
                    member.ZoneId = (ushort)player.Zone.Id;
                    members.ForEach(m =>
                    {
                        IPlayer p = PlayerManager.Instance.GetPlayer(m.CharacterId);
                        if (p == null)
                            return;

                        p.Session.EnqueueMessageEncrypted(new ServerGroupUpdatePlayerRealm
                        {
                            GroupId = Id,
                            TargetPlayerIdentity = new TargetPlayerIdentity() { CharacterId = m.CharacterId, RealmId = RealmContext.Instance.RealmId },
                            MapId = p.Map.Entry.Id,
                            RealmId = RealmContext.Instance.RealmId,
                            PhaseId = 1,
                            IsSyncdToGroup = true,
                            ZoneId = member.ZoneId
                        });
                    });
                }
            }

            var updates = new Dictionary<ushort, ServerGroupPositionUpdate>();
            foreach (var member in members)
            {
                IPlayer player = PlayerManager.Instance.GetPlayer(member.CharacterId);
                if (player == null)
                    continue;

                if (!updates.TryGetValue(member.ZoneId, out ServerGroupPositionUpdate update))
                {
                    update = new ServerGroupPositionUpdate
                    {
                        GroupId = Id,
                        Updates = new List<ServerGroupPositionUpdate.UnknownStruct0>(),
                        WorldId = player.Map.Entry.Id
                    };
                    updates.Add(member.ZoneId, update);
                }

                var entry = new ServerGroupPositionUpdate.UnknownStruct0
                {
                    Identity = new TargetPlayerIdentity() { CharacterId = member.CharacterId, RealmId = RealmContext.Instance.RealmId },
                    Flags = 0,
                    Position = new Position(player.Position),
                    Unknown0 = 0
                };
                update.Updates.Add(entry);
            }

            foreach (var item in updates)
            {
                members.ForEach(m =>
                {
                    IPlayer p = PlayerManager.Instance.GetPlayer(m.CharacterId);
                    if (p == null)
                        return;

                    p.Session.EnqueueMessageEncrypted(item.Value);
                });
            }
        }

        /// <summary>
        /// Creates a new Open world Group.
        /// </summary>
        /// <param name="id">The identifier of the Group.</param>
        /// <param name="leader">The Leader of the Group</param>
        /// <returns>The newly created group.</returns>
        public static IGroup CreateOpenWorld(ulong id, IPlayer leader)
        {
            IGroup g = new Group(id);
            g.Flags |= GroupFlags.OpenWorld;
            g.Leader = g.CreateMember(leader);
            return g;
        }

        /// <summary>
        /// Creates a new Instance Group.
        /// </summary>
        /// <param name="id">The identifier of the Group.</param>
        /// <param name="leader">The Leader of the Group</param>
        /// <returns>The newly created group.</returns>
        public static IGroup CreateInstance(ulong id, IPlayer leader)
        {
            IGroup g = new Group(id);
            g.Leader = g.CreateMember(leader);
            return g;
        }

        public static void SendGroupResult(IGameSession session, GroupResult result, ulong groupId = 0, string targetPlayerName = "")
        {
            session.EnqueueMessageEncrypted(new ServerGroupInviteResult
            {
                GroupId = groupId,
                Name = targetPlayerName,
                Result = result
            });
        }
    }
}
