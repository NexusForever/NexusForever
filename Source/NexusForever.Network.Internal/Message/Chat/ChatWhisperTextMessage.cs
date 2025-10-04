using NexusForever.Network.Internal.Message.Chat.Shared;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Chat
{
    public class ChatWhisperTextMessage
    {
        public Identity Sender { get; set; }
        public IdentityName SenderName { get; set; }
        public Identity Recipient { get; set; }
        public IdentityName RecipientName { get; set; }
        public ChatChannelText Text { get; set; }
        public bool IsAccountWhisper { get; set; }
    }
}
