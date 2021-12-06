using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupInviteResponseHandler : IMessageHandler<IWorldSession, ClientGroupInviteResponse>
    {
        public void HandleMessage(IWorldSession session, ClientGroupInviteResponse groupInviteResponse)
        {
            IGroup joinedGroup = GroupManager.Instance.GetGroupById(groupInviteResponse.GroupId);
            if (joinedGroup == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.GroupNotFound, groupInviteResponse.GroupId, session.Player.Name);
                return;
            }

            // Check if the targeted player declined the group invite.
            if (groupInviteResponse.Result == GroupInviteResult.Declined)
            {
                joinedGroup.DeclineInvite(session.Player.GroupInvite);
                return;
            }

            // Check if the Player can join the group
            if (!joinedGroup.CanJoinGroup(out GroupResult result))
            {
                GroupHelper.SendGroupResult(session, result, joinedGroup.Id, session.Player.Name);
                return;
            }

            joinedGroup.AcceptInvite(session.Player.GroupInvite);
        }
    }
}
