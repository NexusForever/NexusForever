namespace NexusForever.GameTable.Model
{
    public class ChatChannelEntry
    {
        public uint Id { get; set; }
        public string EnumName { get; set; }
        public string UniversalCommand00 { get; set; }
        public string UniversalCommand01 { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdCommand { get; set; }
        public uint LocalizedTextIdAbbreviation { get; set; }
        public uint LocalizedTextIdAlternate00 { get; set; }
        public uint LocalizedTextIdAlternate01 { get; set; }
        public uint Flags { get; set; }
    }
}
