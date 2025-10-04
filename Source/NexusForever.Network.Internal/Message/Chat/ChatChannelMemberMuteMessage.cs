using NexusForever.Game.Static.Social;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Chat
{
    public class ChatChannelMemberMuteMessage
    {
        public Identity Source { get; set; }
        public ChatChannelType Type { get; set; }
        public ulong? ChatId { get; set; }
        public IdentityName Target { get; set; }
        public bool Set { get; set; }
    }
}
