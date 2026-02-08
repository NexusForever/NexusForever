using NexusForever.Network.Internal.Message.Chat.Shared;

namespace NexusForever.Network.Internal.Message.Chat
{
    public class ChatChannelMembersMessage
    {
        public ChatChannel ChatChannel { get; set; }
        public ChatChannelMember Member { get; set; }
    }
}
