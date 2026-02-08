namespace NexusForever.Database.Chat.Model
{
    public class CharacterChatChannelModel
    {
        public ulong CharacterId { get; set; }
        public ushort RealmId { get; set; }
        public ulong ChatId { get; set; }

        public CharacterModel Character { get; set; }
        public ChatChannelMemberModel Member { get; set; }
    }
}
