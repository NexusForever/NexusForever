namespace NexusForever.Database.Character.Model
{
    public class GuildMemberModel
    {
        public ulong Id { get; set; }
        public ulong CharacterId { get; set; }
        public byte Rank { get; set; }
        public string Note { get; set; }

        public GuildModel Guild { get; set; }
    }
}
