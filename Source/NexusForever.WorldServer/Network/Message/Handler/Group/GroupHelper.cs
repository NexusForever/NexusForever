using NexusForever.Game.Static.Group;
using NexusForever.Network;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    // TODO: rawaho, this needs to moved on the rework
    public static class GroupHelper
    {
        /// <summary>
        /// Send <see cref="ServerGroupInviteResult"/> to the current <see cref="WorldSession"/>
        /// </summary>
        public static void SendGroupResult(IWorldSession session, GroupResult result, ulong groupId = 0, string targetPlayerName = "")
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
        public static void AssertGroupId(IWorldSession session, ulong recievedGroupId, bool assertPrimaryGroup = true)
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
        public static void AssertPermission(IWorldSession session, ulong groupID, GroupMemberInfoFlags action)
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
        public static void AssertGroupLeader(IWorldSession session, ulong groupID)
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
    }
}
