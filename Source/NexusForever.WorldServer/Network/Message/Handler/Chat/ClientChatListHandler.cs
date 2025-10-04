using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Chat
{
    public class ClientChatListHandler : IMessageHandler<IWorldSession, ClientChatList>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientChatListHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatList chatList)
        {
            messagePublisher.PublishAsync(new ChatChannelMembersRequestMessage
            {
                Identity = session.Player.Identity.ToInternalIdentity(),
                Type     = chatList.Channel.Type,
                ChatId   = chatList.Channel.ChatId != 0 ? chatList.Channel.ChatId : null
            }).FireAndForgetAsync();
        }
    }
}
