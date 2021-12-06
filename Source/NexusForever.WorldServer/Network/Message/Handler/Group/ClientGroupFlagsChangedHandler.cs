using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupFlagsChangedHandler : IMessageHandler<IWorldSession, ClientGroupFlagsChanged>
    {
        #region Dependency Injection

        private readonly IGroupManager groupManager;

        public ClientGroupFlagsChangedHandler(
            IGroupManager groupManager)
        {
            this.groupManager = groupManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupFlagsChanged groupFlagsChanged)
        {
            GroupHelper.AssertGroupId(session, groupFlagsChanged.GroupId);
            GroupHelper.AssertGroupLeader(session, groupFlagsChanged.GroupId);

            IGroup group = groupManager.GetGroupById(groupFlagsChanged.GroupId);
            if (group == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.GroupNotFound, groupFlagsChanged.GroupId, session.Player.Name);
                return;
            }

            group.SetGroupFlags(groupFlagsChanged.NewFlags);
        }
    }
}
