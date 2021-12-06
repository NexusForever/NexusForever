using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupRequestJoinResponseHandler : IMessageHandler<IWorldSession, ClientGroupRequestJoinResponse>
    {
        #region Dependency Injection

        private readonly IGroupManager groupManager;

        public ClientGroupRequestJoinResponseHandler(
            IGroupManager groupManager)
        {
            this.groupManager = groupManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupRequestJoinResponse groupRequestJoinResponse)
        {
            // This comes from the leader / assist of the group, assert they are part of the correct group.
            GroupHelper.AssertGroupId(session, groupRequestJoinResponse.GroupId);

            IGroup group = groupManager.GetGroupById(groupRequestJoinResponse.GroupId);
            if (group == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.GroupNotFound);
                return;
            }

            if (groupRequestJoinResponse.AcceptedRequest)
                group.AcceptInvite(groupRequestJoinResponse.InviteeName);
            else
                group.DeclineInvite(groupRequestJoinResponse.InviteeName);
        }
    }
}
