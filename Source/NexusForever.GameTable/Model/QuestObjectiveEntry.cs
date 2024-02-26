namespace NexusForever.GameTable.Model
{
    public class QuestObjectiveEntry
    {
        public uint Id { get; set; }
        public uint Type { get; set; }
        public uint Flags { get; set; }
        public uint Data { get; set; }
        public uint Count { get; set; }
        public uint LocalizedTextIdFull { get; set; }
        public uint WorldLocationsIdIndicator00 { get; set; }
        public uint WorldLocationsIdIndicator01 { get; set; }
        public uint WorldLocationsIdIndicator02 { get; set; }
        public uint WorldLocationsIdIndicator03 { get; set; }
        public uint MaxTimeAllowedMS { get; set; }
        public uint LocalizedTextIdShort { get; set; }
        public uint TargetGroupIdRewardPane { get; set; }
        public uint QuestDirectionId { get; set; }
    }
}
