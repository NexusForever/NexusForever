using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupInviteResponseHandler : IMessageHandler<IWorldSession, ClientGroupInviteResponse>
    {
        public void HandleMessage(IWorldSession session, ClientGroupInviteResponse inviteeResponse)
        {
            IPlayer invitee = session.Player;

            if (invitee.GroupInvite == null)
            {
                return;
            }

            IGroup invitingGroup = invitee.GroupInvite.Group;
            if (invitingGroup == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.GroupNotFound, 0, invitee.Name);
                return;
            }

            // Check if the targeted player declined the group invite.
            if (inviteeResponse.Result == GroupInviteResponse.Declined)
            {
                invitingGroup.DeclineInvite(invitee.GroupInvite);
                return;
            }

            // Check if the Player can join the group
            if (!invitingGroup.CanJoinGroup(out GroupResult result))
            {
                GroupHelper.SendGroupResult(session, result, invitingGroup.Id, invitee.Name);
                return;
            }

            invitingGroup.AcceptInvite(invitee.GroupInvite);
        }
    }
}


