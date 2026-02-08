using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Chat
{
    public class ChatTextAcceptedMessage
    {
        public Identity Source { get; set; }
        public ushort ChatMessageId { get; set; }
        public Identity Target { get; set; }
        public IdentityName TargetName { get; set; }
    }
}
