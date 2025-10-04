using NexusForever.Network.Internal.Message.Chat.Shared;

namespace NexusForever.Network.Internal.Message.Chat
{
    public class ChatChannelMemberAddedMessage
    {
        public ChatChannel ChatChannel { get; set; }
        public ChatChannelMember Member { get; set; }
    }
}
