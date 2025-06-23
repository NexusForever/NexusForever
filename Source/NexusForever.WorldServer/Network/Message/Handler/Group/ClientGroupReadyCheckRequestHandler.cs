using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupReadyCheckRequestHandler : IMessageHandler<IWorldSession, ClientGroupReadyCheckRequest>
    {
        #region Dependency Injection

        private readonly IGroupManager groupManager;

        public ClientGroupReadyCheckRequestHandler(
            IGroupManager groupManager)
        {
            this.groupManager = groupManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupReadyCheckRequest readyCheckRequest)
        {
            GroupHelper.AssertGroupId(session, readyCheckRequest.GroupId);

            IGroup group = GroupManager.Instance.GetGroupById(readyCheckRequest.GroupId);
            if (group == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.GroupNotFound, readyCheckRequest.GroupId, session.Player.Name);
                return;
            }

            if (group.IsRaid && !session.Player.GroupMembershipForeground.IsPartyLeader)
                GroupHelper.AssertPermission(session, group.Id, GroupMemberInfoFlags.CanReadyCheck);
            else
                GroupHelper.AssertGroupLeader(session, group.Id);

            group.PrepareForReadyCheck();
            group.PerformReadyCheck(session.Player, readyCheckRequest.Message);
        }
    }
}
