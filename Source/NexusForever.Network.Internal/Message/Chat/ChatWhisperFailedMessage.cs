using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Chat
{
    public class ChatWhisperFailedMessage
    {
        public Identity Sender { get; set; }
        public IdentityName Recipient { get; set; }
        public ushort ChatMessageId { get; set; }
        public bool IsAccountWhisper { get; set; }
    }
}
