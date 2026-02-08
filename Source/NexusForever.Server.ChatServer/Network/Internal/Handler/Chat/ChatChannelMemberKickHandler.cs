using NexusForever.Database.Chat;
using NexusForever.Game.Static.Chat;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Server.ChatServer.Chat;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Chat
{
    public class ChatChannelMemberKickHandler : IHandleMessages<ChatChannelMemberKickMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _chatContext;
        private readonly OutboxMessagePublisher _messagePublisher;
        private readonly ChatChannelManager _chatChannelManager;

        public ChatChannelMemberKickHandler(
            ChatContext chatContext,
            OutboxMessagePublisher messagePublisher,
            ChatChannelManager chatChannelManager)
        {
            _chatContext        = chatContext;
            _messagePublisher   = messagePublisher;
            _chatChannelManager = chatChannelManager;
        }

        #endregion

        public async Task Handle(ChatChannelMemberKickMessage message)
        {
            ChatResult result = await KickAsync(message);
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

        private async Task<ChatResult> KickAsync(ChatChannelMemberKickMessage message)
        {
            if (message.ChatId == null)
                return ChatResult.DoesntExist;

            ChatChannel channel = await _chatChannelManager.GetChatChannelAsync(message.ChatId.Value);
            if (channel == null)
                return ChatResult.DoesntExist;

            return await channel.KickMemberAsync(message.Source.ToChatIdentity(), message.Target.ToChatIdentity());
        }
    }
}
