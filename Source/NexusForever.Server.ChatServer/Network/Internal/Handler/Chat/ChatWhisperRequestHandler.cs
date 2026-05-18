using NexusForever.Database.Chat;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Server.ChatServer.Character;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Chat
{
    public class ChatWhisperRequestHandler : IHandleMessages<ChatWhisperRequestMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _context;
        private readonly OutboxMessagePublisher _messagePublisher;
        private readonly CharacterManager _characterManager;

        public ChatWhisperRequestHandler(
            ChatContext context,
            OutboxMessagePublisher messagePublisher,
            CharacterManager characterManager)
        {
            _context          = context;
            _messagePublisher = messagePublisher;
            _characterManager = characterManager;
        }

        #endregion

        public async Task Handle(ChatWhisperRequestMessage message)
        {
            bool result = await SendWhisperAsync(message);
            if (!result)
            {
                await _messagePublisher.PublishUrgentAsync(new ChatWhisperFailedMessage
                {
                    Sender           = message.Sender,
                    Recipient        = message.Recipient,
                    ChatMessageId    = message.ChatMessageId,
                    IsAccountWhisper = message.IsAccountWhisper
                });
            }

            await _context.SaveChangesAsync();
            await _messagePublisher.FlushUrgentMessages();
        }

        private async Task<bool> SendWhisperAsync(ChatWhisperRequestMessage message)
        {
            Character.Character sender = await _characterManager.GetCharacterRemoteAsync(message.Sender.ToChatIdentity());
            if (sender == null)
                return false;

            IdentityName recipientIdentity;
            if (string.IsNullOrWhiteSpace(message.Recipient.RealmName))
            {
                recipientIdentity = new IdentityName
                {
                    Name      = message.Recipient.Name,
                    RealmName = sender.IdentityName.RealmName
                };
            }
            else
                recipientIdentity = message.Recipient.ToChatIdentity();

            Character.Character recipient = await _characterManager.GetCharacterRemoteAsync(recipientIdentity);
            if (recipient == null)
                return false;

            if (sender.Identity == recipient.Identity)
                return false;

            if (!recipient.IsOnline)
                return false;

            if (sender.Faction != recipient.Faction)
                return false;

            await _messagePublisher.PublishUrgentAsync(new ChatTextAcceptedMessage
            {
                Source        = message.Sender,
                Target        = recipient.Identity.ToInternalIdentity(),
                TargetName    = recipient.IdentityName.ToInternalIdentity(),
                ChatMessageId = message.ChatMessageId
            });

            await _messagePublisher.PublishAsync(new ChatWhisperTextMessage
            {
                Sender           = sender.Identity.ToInternalIdentity(),
                SenderName       = sender.IdentityName.ToInternalIdentity(),
                Recipient        = recipient.Identity.ToInternalIdentity(),
                RecipientName    = recipient.IdentityName.ToInternalIdentity(),
                Text             = message.Text,
                IsAccountWhisper = message.IsAccountWhisper
            });

            return true;
        }
    }
}
