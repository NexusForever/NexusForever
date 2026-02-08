using NexusForever.Network.Internal.Message.Chat.Shared.Format;

namespace NexusForever.Network.Internal.Message.Chat.Shared
{
    public class ChatChannelText
    {
        public string Text { get; set; }
        public List<ChatChannelTextFormat> Format { get; set; } = [];
    }
}
