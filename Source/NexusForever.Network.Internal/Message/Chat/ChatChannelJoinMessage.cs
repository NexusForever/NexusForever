using NexusForever.Game.Static.Chat;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Chat
{
    public class ChatChannelJoinMessage
    {
        public Identity Identity { get; set; }
        public ChatChannelType Type { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public uint Order { get; set; }
    }
}
