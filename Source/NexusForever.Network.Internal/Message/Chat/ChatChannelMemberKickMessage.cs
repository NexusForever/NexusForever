using NexusForever.Game.Static.Chat;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Chat
{
    public class ChatChannelMemberKickMessage
    {
        public Identity Source { get; set; }
        public ChatChannelType Type { get; set; }
        public ulong? ChatId { get; set; }
        public IdentityName Target { get; set; }
    }
}
