using NexusForever.Game.Static.Social;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Chat
{
    public class ChatChannelResultMessage
    {
        public Identity Identity { get; set; }
        public ChatChannelType Type { get; set; }
        public ulong? ChatId { get; set; }
        public ChatResult Result { get; set; }
    }
}
