namespace NexusForever.Database.Chat.Model
{
    public class ChatChannelOwnerModel
    {
        public ulong ChatId { get; set; }
        public ulong CharacterId { get; set; }
        public ushort RealmId { get; set; }

        public ChatChannelModel ChatChannel { get; set; }
        public ChatChannelMemberModel Member { get; set; }
    }
}
