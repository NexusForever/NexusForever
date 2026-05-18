using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Chat
{
    public class ChatChannelTextAcceptedMessage
    {
        public Identity Identity { get; set; }
        public ushort ChatMessageId { get; set; }
    }
}
