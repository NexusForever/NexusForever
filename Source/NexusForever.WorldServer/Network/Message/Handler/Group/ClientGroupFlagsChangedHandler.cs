using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupFlagsChangedHandler : IMessageHandler<IWorldSession, ClientGroupFlagsChanged>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientGroupFlagsChangedHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupFlagsChanged groupFlagsChanged)
        {
            messagePublisher.PublishAsync(new GroupFlagsUpdateMessage
            {
                GroupId  = groupFlagsChanged.GroupId,
                Identity = session.Player.Identity.ToInternalIdentity(),
                Flags    = groupFlagsChanged.NewFlags
            });
        }
    }
}
