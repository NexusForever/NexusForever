using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Entity;
using NexusForever.Game.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class GroupHandler
    {
        /// <summary>
        /// Send <see cref="ServerGroupInviteResult"/> to the current <see cref="WorldSession"/>
        /// </summary>
        public static void SendGroupResult(WorldSession session, GroupResult result, ulong groupId = 0, string targetPlayerName = "")
        {
            session.EnqueueMessageEncrypted(new ServerGroupInviteResult
            {
                GroupId = groupId,
                Name = targetPlayerName,
                Result = result
            });
        }

        /// <summary>
        /// Asserts that the GroupId recieved from the client is a <see cref="Group"/> the Player is a member of.
        /// </summary>
        /// <param name="session">The Players current <see cref="WorldSession"/></param>
        /// <param name="recievedGroupId">The identifier of the group the requiest is for.</param>
        /// <param name="assertPrimaryGroup">if true; asserts that the supplied Group Id is for the primary group when the player is a member of two groups.</param>
        public static void AssertGroupId(WorldSession session, ulong recievedGroupId, bool assertPrimaryGroup = true)
        {
            // If the player is not part of a Group1 they cannot be part of a Group2 so no need to check.
            if (session.Player.GroupMembership1 == null || session.Player.GroupMembership1.Group == null)
                throw new InvalidPacketValueException();

            ulong sessionGroupId = session.Player.GroupMembership1.Group.Id;
            if (sessionGroupId != recievedGroupId && assertPrimaryGroup)
                throw new InvalidPacketValueException("Player does not belong to the group they wish to perform the action on.");

            if (recievedGroupId != session.Player.GroupMembership1.Group.Id && recievedGroupId != session.Player.GroupMembership2?.Group?.Id)
                throw new InvalidPacketValueException("Player does not belong to the group they wish to perform the action on.");
        }

        /// <summary>
        /// Asserts that the <see cref="Player"/> session group member can perform the requested action.
        /// </summary>
        public static void AssertPermission(WorldSession session, ulong groupID, GroupMemberInfoFlags action)
        {
            if (session.Player.GroupMembership1.Group.Id == groupID)
            {
                if (!session.Player.GroupMembership1.Flags.HasFlag(action))
                    throw new InvalidPacketValueException("Player does not have the Group Role required to perform that action.");
            }
            else if (session.Player.GroupMembership2.Group.Id == groupID)
            {
                if (!session.Player.GroupMembership2.Flags.HasFlag(action))
                    throw new InvalidPacketValueException("Player does not have the Group Role required to perform that action.");
            }
        }

        /// <summary>
        /// Asserts that the <see cref="Player"/> session group member can perform the requested action.
        /// </summary>
        public static void AssertGroupLeader(WorldSession session, ulong groupID)
        {
            if (session.Player.GroupMembership1.Group.Id == groupID)
            {
                if (!session.Player.GroupMembership1.IsPartyLeader)
                    throw new InvalidPacketValueException("Player must be the leader of the group to perform this action.");
            }
            else if (session.Player.GroupMembership2.Group.Id == groupID)
            {
                if (!session.Player.GroupMembership2.IsPartyLeader)
                    throw new InvalidPacketValueException("Player must be the leader of the group to perform this action.");
            }
        }

        [MessageHandler(GameMessageOpcode.ClientGroupInvite)]
        public static void HandleGroupInvite(WorldSession session, ClientGroupInvite groupInvite)
        {
            IPlayer targetedPlayer = PlayerManager.Instance.GetPlayer(groupInvite.Name);
            if (targetedPlayer == null)
            {
                SendGroupResult(session, GroupResult.PlayerNotFound, targetPlayerName: groupInvite.Name);
                return;
            }

            // Check if targeted player is already grouped in a Group1 they cannot be re-invited, only instance finder can create an instance group.
            if (targetedPlayer.GroupMembership1 != null)
            {
                SendGroupResult(session, GroupResult.Grouped, targetPlayerName: groupInvite.Name);
                return;
            }

            // Check if inviter faction is same as invited faction.
            if (targetedPlayer.Faction1 != session.Player.Faction1)
            {
                SendGroupResult(session, GroupResult.WrongFaction, targetPlayerName: groupInvite.Name);
                return;
            }

            // Player is already being invited by another group/player
            if (targetedPlayer.GroupInvite != null)
            {
                SendGroupResult(session, GroupResult.Pending, targetPlayerName: groupInvite.Name);
                return;
            }

            // Check if the inviter is not inviting himself (pleb)
            if (targetedPlayer.Session == session)
            {
                SendGroupResult(session, GroupResult.NotInvitingSelf, targetPlayerName: groupInvite.Name);
                return;
            }

            if (session.Player.GroupMembership1 == null)
            {
                // Player is not part of a group - lets create a new one and invite the new guy.
                IGroup newGroup = GroupManager.Instance.CreateGroup(session.Player);
                newGroup.Invite(session.Player, targetedPlayer);
                return;
            }

            // At this point, the player must be part of a group
            IGroup group = session.Player.GroupMembership1.Group;
            IGroupMember membership = session.Player.GroupMembership1;

            if (group.IsFull)
            {
                SendGroupResult(session, GroupResult.Full, group.Id, groupInvite.Name);
                return;
            }

            // The inviter is the Leader or has Invite permissions, so just do an invite.
            if (group.Leader.Id == membership.Id || membership.Flags.HasFlag(GroupMemberInfoFlags.CanInvite))
                group.Invite(session.Player, targetedPlayer);
            else // inviter is another group memeber w/o invite permissions, so we create a referal.
                group.ReferMember(membership, targetedPlayer);
        }

        [MessageHandler(GameMessageOpcode.ClientGroupRequestJoin)]
        public static void HandleJoinGroupRequest(WorldSession session, ClientGroupRequestJoin joinRequest)
        {
            if (session.Player.GroupMembership1 != null) // player who did /join is already in a group. This has no effect.
                return;

            IPlayer targetedPlayer = PlayerManager.Instance.GetPlayer(joinRequest.Name);
            if (targetedPlayer == null)
                return;

            if (targetedPlayer.GroupMembership1 == null)
            {
                // Player and Target are not part of a group - create one for them both so /join acts as /invite.
                IGroup newGroup = GroupManager.Instance.CreateGroup(session.Player);
                newGroup.Invite(session.Player, targetedPlayer);
            }
            else
            {
                IGroup group = targetedPlayer.GroupMembership1.Group;
                if (targetedPlayer.GroupMembership1.IsPartyLeader)  // /Join was on the leader - so just do a std Join request.
                    group.HandleJoinRequest(session.Player);
                else  //target player is not the leader of the group, so this acts as a referral
                    group.ReferMember(session.Player.GroupMembership1, targetedPlayer);
            }
        }

        [MessageHandler(GameMessageOpcode.ClientGroupRequestJoinResponse)]
        public static void HandleClientGroupRequestJoinResponse(WorldSession session, ClientGroupRequestJoinResponse clientGroupRequestJoinResponse)
        {
            // This comes from the leader / assist of the group, assert they are part of the correct group.
            AssertGroupId(session, clientGroupRequestJoinResponse.GroupId);

            IGroup group = GroupManager.Instance.GetGroupById(clientGroupRequestJoinResponse.GroupId);
            if (group == null)
            {
                SendGroupResult(session, GroupResult.GroupNotFound);
                return;
            }

            if (clientGroupRequestJoinResponse.AcceptedRequest)
                group.AcceptInvite(clientGroupRequestJoinResponse.InviteeName);
            else
                group.DeclineInvite(clientGroupRequestJoinResponse.InviteeName);
        }

        [MessageHandler(GameMessageOpcode.ClientGroupInviteResponse)]
        public static void HandleGroupInviteResponse(WorldSession session, ClientGroupInviteResponse response)
        {
            IGroup joinedGroup = GroupManager.Instance.GetGroupById(response.GroupId);
            if (joinedGroup == null)
            {
                SendGroupResult(session, GroupResult.GroupNotFound, response.GroupId, session.Player.Name);
                return;
            }

            // Check if the targeted player declined the group invite.
            if (response.Result == GroupInviteResult.Declined)
            {
                joinedGroup.DeclineInvite(session.Player.GroupInvite);
                return;
            }

            // Check if the Player can join the group
            if (!joinedGroup.CanJoinGroup(out GroupResult result))
            {
                SendGroupResult(session, result, joinedGroup.Id, session.Player.Name);
                return;
            }

            joinedGroup.AcceptInvite(session.Player.GroupInvite);
        }

        [MessageHandler(GameMessageOpcode.ClientGroupKick)]
        public static void HandleGroupKick(WorldSession session, ClientGroupKick kick)
        {
            AssertGroupId(session, kick.GroupId);
            AssertPermission(session, kick.GroupId, GroupMemberInfoFlags.CanKick);

            IGroup group = GroupManager.Instance.GetGroupById(kick.GroupId);
            if (group == null)
            {
                SendGroupResult(session, GroupResult.GroupNotFound, kick.GroupId, session.Player.Name);
                return;
            }

            // I never want to leave a group with only 1 member; So as with the Leave if there would be 1 member left after this operation
            // Just .Disband() the group.
            // TODO: If WoW is anything to go by; instance groups do NOT disband like this; once the instance is closed the group will be cleaned up.
            if (group.MemberCount == 2 && group.IsOpenWorld)
                group.Disband();
            else
                group.KickMember(kick.TargetedPlayer);
        }

        [MessageHandler(GameMessageOpcode.ClientGroupLeave)]
        public static void HandleGroupLeave(WorldSession session, ClientGroupLeave leave)
        {
            AssertGroupId(session, leave.GroupId);

            IGroup group = GroupManager.Instance.GetGroupById(leave.GroupId);
            if (group == null)
            {
                SendGroupResult(session, GroupResult.GroupNotFound, leave.GroupId, session.Player.Name);
                return;
            }

            // I never want to leave a group with only 1 member; So as with the Kick if there would be 1 member left after this operation
            // Just .Disband() the group.
            // TODO: If WoW is anything to go by; instance groups do NOT disband like this; once the instance is closed the group will be cleaned up.
            if (leave.ShouldDisband || group.MemberCount == 2 && group.IsOpenWorld)
            {
                group.Disband();
                return;
            }

            //TODO: This may not be correct? I need to look into if i can leave my main group whilst part of an instance group.
            group.RemoveMember(session.Player.GroupMembership1);
        }

        [MessageHandler(GameMessageOpcode.ClientGroupMarkUnit)]
        public static void HandleGroupMarkUnit(WorldSession session, ClientGroupMark clientMark)
        {
            // Players can only mark for their Active group.
            ulong groupId = session.Player.GroupMembership1.Group.Id;
            IGroup group = GroupManager.Instance.GetGroupById(groupId);
            if (group == null)
            {
                SendGroupResult(session, GroupResult.GroupNotFound, groupId, session.Player.Name);
                return;
            }

            AssertPermission(session, groupId, GroupMemberInfoFlags.CanMark);
            group.MarkUnit(clientMark.UnitId, clientMark.Marker);
        }

        [MessageHandler(GameMessageOpcode.ClientGroupFlagsChanged)]
        public static void HandleGroupFlagsChanged(WorldSession session, ClientGroupFlagsChanged clientGroupFlagsChanged)
        {
            AssertGroupId(session, clientGroupFlagsChanged.GroupId);
            AssertGroupLeader(session, clientGroupFlagsChanged.GroupId);

            IGroup group = GroupManager.Instance.GetGroupById(clientGroupFlagsChanged.GroupId);
            if (group == null)
            {
                SendGroupResult(session, GroupResult.GroupNotFound, clientGroupFlagsChanged.GroupId, session.Player.Name);
                return;
            }

            group.SetGroupFlags(clientGroupFlagsChanged.NewFlags);
        }

        [MessageHandler(GameMessageOpcode.ClientGroupSetRole)]
        public static void HandleGroupSetRole(WorldSession session, ClientGroupSetRole clientGroupSetRole)
        {
            AssertGroupId(session, clientGroupSetRole.GroupId);

            IGroup group = GroupManager.Instance.GetGroupById(clientGroupSetRole.GroupId);
            if (group == null)
            {
                SendGroupResult(session, GroupResult.GroupNotFound, clientGroupSetRole.GroupId, session.Player.Name);
                return;
            }

            group.UpdateMemberRole(session.Player.GroupMembership1, clientGroupSetRole.TargetedPlayer, clientGroupSetRole.ChangedFlag, clientGroupSetRole.CurrentFlags.HasFlag(clientGroupSetRole.ChangedFlag));
        }

        [MessageHandler(GameMessageOpcode.ClientGroupSendReadyCheck)]
        public static void HandleSendReadyCheck(WorldSession session, ClientGroupSendReadyCheck sendReadyCheck)
        {
            AssertGroupId(session, sendReadyCheck.GroupId);

            IGroup group = GroupManager.Instance.GetGroupById(sendReadyCheck.GroupId);
            if (group == null)
            {
                SendGroupResult(session, GroupResult.GroupNotFound, sendReadyCheck.GroupId, session.Player.Name);
                return;
            }

            if (group.IsRaid && !session.Player.GroupMembership1.IsPartyLeader)
                AssertPermission(session, group.Id, GroupMemberInfoFlags.CanReadyCheck);
            else
                AssertGroupLeader(session, group.Id);

            group.PrepareForReadyCheck();
            group.PerformReadyCheck(session.Player, sendReadyCheck.Message);
        }

        [MessageHandler(GameMessageOpcode.ClientGroupLootRulesChange)]
        public static void HandleGroupLootRulesChange(WorldSession session, ClientGroupLootRulesChange clientGroupLootRulesChange)
        {
            AssertGroupId(session, clientGroupLootRulesChange.GroupId);
            AssertGroupLeader(session, clientGroupLootRulesChange.GroupId);

            IGroup group = GroupManager.Instance.GetGroupById(clientGroupLootRulesChange.GroupId);
            group.UpdateLootRules(clientGroupLootRulesChange.LootRulesUnderThreshold, clientGroupLootRulesChange.LootRulesThresholdAndOver, clientGroupLootRulesChange.Threshold, clientGroupLootRulesChange.HarvestingRule);
        }

        [MessageHandler(GameMessageOpcode.ClientGroupPromote)]
        public static void ClientGroupPromote(WorldSession session, ClientGroupPromote clientGroupPromote)
        {
            AssertGroupId(session, clientGroupPromote.GroupId);

            IGroup group = GroupManager.Instance.GetGroupById(clientGroupPromote.GroupId);
            group.Promote(clientGroupPromote.TargetedPlayer);
        }
    }
}
