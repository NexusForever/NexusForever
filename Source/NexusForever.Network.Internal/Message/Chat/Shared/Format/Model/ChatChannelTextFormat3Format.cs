using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format.Model
{
    public class ChatChannelTextFormat3Format : IChatChannelTextFormatModel
    {
        public ChatFormatType Type => ChatFormatType.Format3;
        public bool Unknown { get; set; }
    }
}
