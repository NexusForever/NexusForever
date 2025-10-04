using NexusForever.Game.Static.Social;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format
{
    public class ChatChannelTextFormat
    {
        public ChatFormatType Type { get; set; }
        public ushort StartIndex { get; set; }
        public ushort StopIndex { get; set; }
        public IChatChannelTextFormatModel Model { get; set; }
    }
}
