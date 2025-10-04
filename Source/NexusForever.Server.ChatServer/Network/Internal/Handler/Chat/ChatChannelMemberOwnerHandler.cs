using NexusForever.Database.Chat;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Server.ChatServer.Chat;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Chat
{
    public class ChatChannelMemberOwnerHandler : IHandleMessages<ChatChannelMemberOwnerMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _chatContext;
        private readonly OutboxMessagePublisher _messagePublisher;
        private readonly ChatChannelManager _chatChannelManager;

        public ChatChannelMemberOwnerHandler(
            ChatContext chatContext,
            OutboxMessagePublisher messagePublisher,
            ChatChannelManager chatChannelManager)
        {
            _chatContext        = chatContext;
            _messagePublisher   = messagePublisher;
            _chatChannelManager = chatChannelManager;
        }

        #endregion

        public async Task Handle(ChatChannelMemberOwnerMessage message)
        {
            ChatResult result = await SetOwnerAsync(message);
            if (result != ChatResult.Ok)
            {
                await _messagePublisher.PublishUrgentAsync(new ChatChannelResultMessage
                {
                    Identity = message.Source,
                    Type     = message.Type,
                    ChatId   = message.ChatId,
                    Result   = result
                });
            }

            await _chatContext.SaveChangesAsync();
            await _messagePublisher.FlushUrgentMessages();
        }

        private async Task<ChatResult> SetOwnerAsync(ChatChannelMemberOwnerMessage message)
        {
            if (message.ChatId == null)
                return ChatResult.DoesntExist;

            ChatChannel channel = await _chatChannelManager.GetChatChannelAsync(message.ChatId.Value);
            if (channel == null)
                return ChatResult.DoesntExist;

            return await channel.SetOwnerMemberAsync(message.Source.ToChatIdentity(), message.Target.ToChatIdentity());
        }
    }
}
