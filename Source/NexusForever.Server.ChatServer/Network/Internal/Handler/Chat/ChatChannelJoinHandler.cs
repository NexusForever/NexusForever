using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NexusForever.Database.Chat;
using NexusForever.Game.Static.Chat;
using NexusForever.GameTable.Text.Filter;
using NexusForever.GameTable.Text.Static;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Server.ChatServer.Character;
using NexusForever.Server.ChatServer.Chat;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Chat
{
    public class ChatChannelJoinHandler : IHandleMessages<ChatChannelJoinMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _context;
        private readonly CharacterManager _characterManager;
        private readonly ChatChannelManager _chatChannelManager;
        private readonly OutboxMessagePublisher _messagePublisher;
        private readonly ITextFilterManager _textFilterManager;

        public ChatChannelJoinHandler(
            ChatContext context,
            CharacterManager characterManager,
            ChatChannelManager chatChannelManager,
            OutboxMessagePublisher messagePublisher,
            ITextFilterManager textFilterManager)
        {
            _context            = context;
            _characterManager   = characterManager;
            _chatChannelManager = chatChannelManager;
            _messagePublisher   = messagePublisher;
            _textFilterManager  = textFilterManager;
        }

        #endregion

        public async Task Handle(ChatChannelJoinMessage message)
        {
            IExecutionStrategy strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

                ChatResult result = await JoinAsync(message);
                if (result != ChatResult.Ok)
                {
                    await _messagePublisher.PublishUrgentAsync(new ChatChannelJoinResultMessage
                    {
                        Identity = message.Identity,
                        Type     = message.Type,
                        Name     = message.Name,
                        Result   = result
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            });
        }

        private async Task<ChatResult> JoinAsync(ChatChannelJoinMessage message)
        {
            // this handler is only for custom channels
            if (message.Type != ChatChannelType.Custom)
                return ChatResult.Throttled;

            Character.Character character = await _characterManager.GetCharacterRemoteAsync(message.Identity.ToChatIdentity());
            if (character == null)
                return ChatResult.InvalidCharacterName;

            if (character.GetChannelCount() >= 20)
                return ChatResult.TooManyCustomChannels;

            ChatChannel chatChannel = await _chatChannelManager.GetChatChannelAsync(message.Type, message.Name);
            if (chatChannel == null)
            {
                if (!_textFilterManager.IsTextValid(message.Name)
                        || !_textFilterManager.IsTextValid(message.Name, UserText.ChatCustomChannelName))
                    return ChatResult.BadName;

                if (!string.IsNullOrEmpty(message.Password)
                    && (!_textFilterManager.IsTextValid(message.Password)
                    || !_textFilterManager.IsTextValid(message.Password, UserText.ChatCustomChannelPassword)))
                    return ChatResult.BadPassword;

                chatChannel = _chatChannelManager.CreateChatChannel(message.Type, message.Name, message.Password);
                // required to ensure the chat channel has an id assigned before any outbox messages are generated
                await _context.SaveChangesAsync();
            }

            return await chatChannel.JoinAsync(character, message.Password);
        }
    }
}
