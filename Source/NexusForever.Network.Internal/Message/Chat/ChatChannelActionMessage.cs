using NexusForever.Game.Static.Social;

namespace NexusForever.Network.Internal.Message.Chat
{
    public class ChatChannelActionMessage
    {
        public Shared.ChatChannel Channel { get; set; }
        public Shared.ChatChannelMember Source { get; set; }
        public Shared.ChatChannelMember Target { get; set; }
        public ChatChannelAction Action { get; set; }
    }
}
