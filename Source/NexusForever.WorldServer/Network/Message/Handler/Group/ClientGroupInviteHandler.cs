using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Entity;
using NexusForever.Game.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupInviteHandler : IMessageHandler<IWorldSession, ClientGroupInvite>
    {
        /// <summary>
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientGroupInvite groupInvite)
        {
            IPlayer inviter = session.Player;
            IPlayer invitee = PlayerManager.Instance.GetPlayer(groupInvite.InviteeName);

            if (invitee == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.PlayerNotFound, targetPlayerName: groupInvite.InviteeName);
                return;
            }

            // Check if targeted player is already grouped they cannot be re-invited, only instance finder can create an instance group that pushes existing foreground group to background.
            // Though it should never be possible to have only a Background group, checking both to be sure there are no edge cases.
            if (invitee.GroupMembershipForeground != null || invitee.GroupMembershipBackground != null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.Grouped, targetPlayerName: groupInvite.InviteeName);
                return;
            }

            // Check if inviter faction is same as invited faction.
            // Make cross faction groups possible as an option?
            if (invitee.Faction1 != session.Player.Faction1)
            {
                GroupHelper.SendGroupResult(session, GroupResult.WrongFaction, targetPlayerName: groupInvite.InviteeName);
                return;
            }

            // Player is already being invited by another group/player
            if (invitee.GroupInvite != null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.Pending, targetPlayerName: groupInvite.InviteeName);
                return;
            }

            // TODO: Add case for server controlled unit

            // Check if the inviter is not inviting himself (pleb)
            if (invitee.Session == session)
            {
                GroupHelper.SendGroupResult(session, GroupResult.NotInvitingSelf, targetPlayerName: groupInvite.InviteeName);
                return;
            }

            if (inviter.GroupMembershipForeground == null && inviter.GroupMembershipBackground == null)
            {
                // Inviter is not part of a group - lets create a new one and invite the new guy.
                IGroup newGroup = GroupManager.Instance.CreateGroup( inviter);
                newGroup.Invite(inviter, invitee);
                return;
            }
            else
            {
                // Can only invite to the Foreground group
                IGroup group = inviter.GroupMembershipForeground.Group;
                IGroupMember inviterMember = inviter.GroupMembershipForeground;

                if (group.IsFull)
                {
                    GroupHelper.SendGroupResult(session, GroupResult.Full, group.Id, groupInvite.InviteeName);
                    return;
                }

                // The inviter is the Leader or has Invite permissions, so just do an invite.
                if (group.Leader.Identity == inviterMember.Identity || inviterMember.Flags.HasFlag(GroupMemberInfoFlags.CanInvite))
                    group.Invite(inviter, invitee);
                else // inviter is another group memeber w/o invite permissions, so we create a referal.
                    group.ReferMember(inviterMember, invitee);
            }
        }
    }
}


