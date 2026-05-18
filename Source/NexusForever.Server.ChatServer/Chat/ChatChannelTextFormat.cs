using NexusForever.Game.Static.Chat;
using NexusForever.Network.Internal.Message.Chat.Shared.Format;

namespace NexusForever.Server.ChatServer.Chat
{
    public class ChatChannelTextFormat
    {
        public ChatFormatType Type { get; set; }
        public ushort StartIndex { get; set; }
        public ushort StopIndex { get; set; }
        public IChatChannelTextFormatModel Model { get; set; }
    }
}
