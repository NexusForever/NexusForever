using NexusForever.Database.Chat;
using NexusForever.Game.Static.Chat;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Server.ChatServer.Character;
using NexusForever.Server.ChatServer.Chat;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Chat
{
    public class ChatChannelTextRequestHandler : IHandleMessages<ChatChannelTextRequestMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _context;
        private readonly CharacterManager _characterManager;
        private readonly OutboxMessagePublisher _messagePublisher;

        public ChatChannelTextRequestHandler(
            ChatContext context,
            CharacterManager characterManager,
            OutboxMessagePublisher messagePublisher)
        {
            _context          = context;
            _characterManager = characterManager;
            _messagePublisher = messagePublisher;
        }

        #endregion

        public async Task Handle(ChatChannelTextRequestMessage message)
        {
            ChatResult result = await SendTextAsync(message);
            if (result != ChatResult.Ok)
            {
                await _messagePublisher.PublishUrgentAsync(new ChatChannelTextResultMessage
                {
                    Identity      = message.Source,
                    Type          = message.Type,
                    ChatId        = message.ChatId,
                    ChatMessageId = message.ChatMessageId,
                    Result        = result
                });
            }

            await _context.SaveChangesAsync();
            await _messagePublisher.FlushUrgentMessages();
        }

        private async Task<ChatResult> SendTextAsync(ChatChannelTextRequestMessage message)
        {
            Character.Character character = await _characterManager.GetCharacterAsync(message.Source.ToChatIdentity());
            if (character == null)
                return ChatResultNotIn(message.Type);

            ChatChannel chatChannel = await character.GetChatChannelAsync(message.Type);
            if (chatChannel == null)
                return ChatResultNotIn(message.Type);

            return await chatChannel.SendTextAsync(message.Source.ToChatIdentity(), message.Text.ToChat(), message.ChatMessageId);
        }

        private ChatResult ChatResultNotIn(ChatChannelType type)
        {
            return type switch
            {
                ChatChannelType.Party           => ChatResult.NotInGroup,
                ChatChannelType.Instance        => ChatResult.NotInGroup,
                ChatChannelType.Guild           => ChatResult.NotInGuild,
                ChatChannelType.GuildOfficer    => ChatResult.NotGuildOfficer,
                ChatChannelType.Society         => ChatResult.NotInSociety,
                ChatChannelType.WarParty        => ChatResult.NotInWarParty,
                ChatChannelType.WarPartyOfficer => ChatResult.NotWarPartyOfficer,
                _                               => ChatResult.DoesntExist
            };
        }
    }
}
