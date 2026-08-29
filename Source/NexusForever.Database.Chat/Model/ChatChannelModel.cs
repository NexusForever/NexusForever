using NexusForever.Game.Static.Chat;

namespace NexusForever.Database.Chat.Model
{
    public class ChatChannelModel
    {
        public ulong ChatId { get; set; }
        public ChatChannelType Type { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public ChatChannelReferenceType? ReferenceType { get; set; }
        public ulong? ReferenceValue { get; set; }

        public ChatChannelOwnerModel Owner { get; set; }
        public List<ChatChannelMemberModel> Members { get; set; } = [];
    }
}
