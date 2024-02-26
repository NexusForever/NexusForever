namespace NexusForever.GameTable.Model
{
    public class DatacubeEntry
    {
        public uint Id { get; set; }
        public uint DatacubeTypeEnum { get; set; }
        public uint LocalizedTextIdTitle { get; set; }
        public uint LocalizedTextIdText00 { get; set; }
        public uint LocalizedTextIdText01 { get; set; }
        public uint LocalizedTextIdText02 { get; set; }
        public uint LocalizedTextIdText03 { get; set; }
        public uint LocalizedTextIdText04 { get; set; }
        public uint LocalizedTextIdText05 { get; set; }
        public uint SoundEventId { get; set; }
        public uint WorldZoneId { get; set; }
        public uint UnlockCount { get; set; }
        public string AssetPathImage { get; set; }
        public uint DatacubeFactionEnum { get; set; }
        public uint WorldLocation2Id { get; set; }
        public uint QuestDirectionId { get; set; }
    }
}
