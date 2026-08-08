using NexusForever.Game.Static.Chat;

namespace NexusForever.Database.Chat.Model
{
    public class ChatChannelMemberModel
    {
        public ulong ChatId { get; set; }
        public ulong CharacterId { get; set; }
        public ushort RealmId { get; set; }
        public ChatChannelMemberFlags Flags { get; set; }

        public ChatChannelModel ChatChannel { get; set; }
        public CharacterModel Character { get; set; }
    }
}
