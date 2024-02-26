namespace NexusForever.GameTable.Model
{
    public class RandomPlayerNameEntry
    {
        public uint Id { get; set; }
        public string NameFragment { get; set; }
        public uint NameFragmentTypeEnum { get; set; }
        public uint RaceId { get; set; }
        public uint Gender { get; set; }
        public uint Faction2Id { get; set; }
        public uint LanguageFlags { get; set; }
    }
}
