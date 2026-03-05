using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format.Model
{
    public class ChatChannelTextNavPointFormat : IChatChannelTextFormatModel
    {
        public ChatFormatType Type => ChatFormatType.NavPoint;
        public ushort MapZoneId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}
