using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupSendReadyCheckHandler : IMessageHandler<IWorldSession, ClientGroupSendReadyCheck>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientGroupSendReadyCheckHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupSendReadyCheck groupSendReadyCheck)
        {
            messagePublisher.PublishAsync(new GroupReadyCheckMessage
            {
                GroupId  = groupSendReadyCheck.GroupId,
                Initator = session.Player.Identity.ToInternalIdentity(),
                Message  = groupSendReadyCheck.Message
            }).FireAndForgetAsync();
        }
    }
}
