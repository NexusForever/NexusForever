using NexusForever.Game.Abstract.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupPromoteHandler : IMessageHandler<IWorldSession, ClientGroupPromote>
    {
        #region Dependency Injection

        private readonly IGroupManager groupManager;

        public ClientGroupPromoteHandler(
            IGroupManager groupManager)
        {
            this.groupManager = groupManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupPromote groupPromote)
        {
            GroupHelper.AssertGroupId(session, groupPromote.GroupId);

            IGroup group = groupManager.GetGroupById(groupPromote.GroupId);
            group.Promote(groupPromote.TargetedPlayer);
        }
    }
}
