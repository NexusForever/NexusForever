using NexusForever.Game.Static.Social;
using NexusForever.Network.Internal.Message.Chat.Shared;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Chat
{
    public class ChatChannelTextRequestMessage
    {
        public Identity Source { get; set; }
        public ChatChannelType Type { get; set; }
        public ulong? ChatId { get; set; }
        public ChatChannelText Text { get; set; }
        public ushort ChatMessageId { get; set; }
    }
}
