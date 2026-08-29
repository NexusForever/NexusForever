using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.Internal.Message.Chat.Shared
{
    public class ChatChannel
    {
        public ulong ChatId { get; set; }
        public ChatChannelType Type { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public ChatChannelReferenceType? ReferenceType { get; set; }
        public ulong? ReferenceValue { get; set; }

        public List<ChatChannelMember> Members { get; set; }
    }
}
