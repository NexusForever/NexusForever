using System.Threading.Tasks;
using NexusForever.Game.Abstract.Group;
using NexusForever.Network.Internal.Message.Group;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public class GroupDisbandedHandler : IHandleMessages<GroupDisbandedMessage>
    {
        #region Dependency Injection

        private readonly IGroupManager groupManager;

        public GroupDisbandedHandler(
            IGroupManager groupManager)
        {
            this.groupManager = groupManager;
        }

        #endregion

        public Task Handle(GroupDisbandedMessage message)
        {
            groupManager.RemoveGroup(message.Group);
            return Task.CompletedTask;
        }
    }
}
