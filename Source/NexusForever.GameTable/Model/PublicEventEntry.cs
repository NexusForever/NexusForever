namespace NexusForever.GameTable.Model
{
    public class PublicEventEntry
    {
        public uint Id { get; set; }
        public uint WorldId { get; set; }
        public uint WorldZoneId { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint FailureTimeMs { get; set; }
        public uint WorldLocation2Id { get; set; }
        public uint PublicEventTypeEnum { get; set; }
        public uint PublicEventIdParent { get; set; }
        public uint MinPlayerLevel { get; set; }
        public uint LiveEventIdLifetime { get; set; }
        public uint PublicEventFlags { get; set; }
        public uint LocalizedTextIdEnd { get; set; }
        public uint RewardRotationContentId { get; set; }
    }
}
