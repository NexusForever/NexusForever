using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Chat
{
    public class ClientChatLeaveHandler : IMessageHandler<IWorldSession, ClientChatLeave>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientChatLeaveHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatLeave chatLeave)
        {
            messagePublisher.PublishAsync(new ChatChannelMemberLeaveMessage
            {
                Identity = session.Player.Identity.ToInternalIdentity(),
                Type     = chatLeave.Channel.Type,
                ChatId   = chatLeave.Channel.ChatId != 0 ? chatLeave.Channel.ChatId : null
            }).FireAndForgetAsync();
        }
    }
}
