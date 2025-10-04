using NexusForever.Game.Static.Social;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Chat
{
    public class ChatChannelMemberOwnerMessage
    {
        public Identity Source { get; set; }
        public ChatChannelType Type { get; set; }
        public ulong? ChatId { get; set; }
        public IdentityName Target { get; set; }
    }
}
