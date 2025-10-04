using NexusForever.Network.Internal.Message.Chat.Shared;

namespace NexusForever.Network.Internal.Message.Chat
{
    public class ChatChannelTextMessage
    {
        public ChatChannel ChatChannel { get; set; }
        public ChatChannelMember Sender { get; set; }
        public ChatChannelText Text { get; set; }
    }
}
