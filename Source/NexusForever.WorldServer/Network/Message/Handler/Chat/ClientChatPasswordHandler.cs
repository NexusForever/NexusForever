using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Chat;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Chat
{
    public class ClientChatPasswordHandler : IMessageHandler<IWorldSession, ClientChatPassword>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientChatPasswordHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatPassword chatPassword)
        {
            messagePublisher.PublishAsync(new ChatChannelPasswordMessage
            {
                Identity = session.Player.Identity.ToInternalIdentity(),
                Type     = chatPassword.Channel.ChatChannelId,
                ChatId   = chatPassword.Channel.ChatId != 0 ? chatPassword.Channel.ChatId : null,
                Password = chatPassword.Password
            }).FireAndForgetAsync();
        }
    }
}
