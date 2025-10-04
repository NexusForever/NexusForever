using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Chat
{
    public class ClientChatJoinHandler : IMessageHandler<IWorldSession, ClientChatJoin>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientChatJoinHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatJoin chatJoin)
        {
            messagePublisher.PublishAsync(new ChatChannelJoinMessage
            {
                Identity = session.Player.Identity.ToInternalIdentity(),
                Type     = chatJoin.Type,
                Name     = chatJoin.Name,
                Password = chatJoin.Password,
                Order    = chatJoin.Order
            });
        }
    }
}
