using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupJoinReponseHandler : IMessageHandler<IWorldSession, ClientGroupJoinResponse>
    {
        /// <summary>
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientGroupJoinResponse joinResponse)
        {
            // This comes from the leader / assist of the group, assert they are part of the correct group.
            GroupHelper.AssertGroupId(session, joinResponse.GroupId);

            IGroup group = GroupManager.Instance.GetGroupById(joinResponse.GroupId);
            if (group == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.GroupNotFound);
                return;
            }

            if (joinResponse.AcceptedRequest)
                group.AcceptInvite(joinResponse.InviteeName);
            else
                group.DeclineInvite(joinResponse.InviteeName);
        }
    }
}