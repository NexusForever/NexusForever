using NexusForever.Game.Static.Social;

namespace NexusForever.Network.Internal.Message.Chat
{
    public class ChatChannelMemberLeftMessage
    {
        public Shared.ChatChannel Channel { get; set; }
        public Shared.ChatChannelMember Member { get; set; }
        public ChatChannelLeaveReason Reason { get; set; }
    }
}
