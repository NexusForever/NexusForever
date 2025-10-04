using NexusForever.Game.Static.Social;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Chat.Shared
{
    public class ChatChannelMember
    {
        public Identity Identity { get; set; }
        public ChatChannelMemberFlags Flags { get; set; }
        public ChatCharacter Character { get; set; }
    }
}
