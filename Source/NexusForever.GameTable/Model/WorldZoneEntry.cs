namespace NexusForever.GameTable.Model
{
    public class WorldZoneEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint ParentZoneId { get; set; }
        public bool AllowAccess { get; set; }
        public uint Color { get; set; }
        public uint SoundZoneKitId { get; set; }
        public uint WorldLocation2IdExit { get; set; }
        public uint Flags { get; set; }
        public uint ZonePvpRulesEnum { get; set; }
        public uint RewardRotationContentId { get; set; }
    }
}
