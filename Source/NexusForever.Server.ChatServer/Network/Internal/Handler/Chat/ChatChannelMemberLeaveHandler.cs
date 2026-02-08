using NexusForever.Database.Chat;
using NexusForever.Game.Static.Chat;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Server.ChatServer.Chat;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Chat
{
    public class ChatChannelMemberLeaveHandler : IHandleMessages<ChatChannelMemberLeaveMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _chatContext;
        private readonly OutboxMessagePublisher _messagePublisher;
        private readonly ChatChannelManager _chatChannelManager;

        public ChatChannelMemberLeaveHandler(
            ChatContext chatContext,
            OutboxMessagePublisher messagePublisher,
            ChatChannelManager chatChannelManager)
        {
            _chatContext        = chatContext;
            _messagePublisher   = messagePublisher;
            _chatChannelManager = chatChannelManager;
        }

        #endregion

        public async Task Handle(ChatChannelMemberLeaveMessage message)
        {
            ChatResult result = await LeaveAsync(message);
            if (result != ChatResult.Ok)
            {
                await _messagePublisher.PublishUrgentAsync(new ChatChannelResultMessage
                {
                    Identity = message.Identity,
                    Type     = message.Type,
                    ChatId   = message.ChatId,
                    Result   = result
                });
            }

            await _chatContext.SaveChangesAsync();
            await _messagePublisher.FlushUrgentMessages();
        }

        private async Task<ChatResult> LeaveAsync(ChatChannelMemberLeaveMessage message)
        {
            if (message.ChatId == null)
                return ChatResult.DoesntExist;

            ChatChannel channel = await _chatChannelManager.GetChatChannelAsync(message.ChatId.Value);
            if (channel == null)
                return ChatResult.DoesntExist;

            return await channel.LeaveAsync(message.Identity.ToChatIdentity());
        }
    }
}
