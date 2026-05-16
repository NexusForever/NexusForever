using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format.Model
{
    public class ChatChannelTextFormat0Format : IChatChannelTextFormatModel
    {
        public ChatFormatType Type => ChatFormatType.Format0;
        public bool Unknown { get; set; }
    }
}
