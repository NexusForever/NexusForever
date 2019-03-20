namespace NexusForever.Database.Character.Model
{
    public partial class GuildMember
    {
        public ulong Id { get; set; }
        public ulong CharacterId { get; set; }
        public byte Rank { get; set; }
        public string Note { get; set; }

        public Guild IdNavigation { get; set; }
    }
}
