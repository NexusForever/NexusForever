namespace NexusForever.GameTable.Model
{
    public class PathEpisodeEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdSummary { get; set; }
        public uint WorldId { get; set; }
        public uint WorldZoneId { get; set; }
        public uint PathTypeEnum { get; set; }
    }
}
