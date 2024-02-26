namespace NexusForever.GameTable.Model
{
    public class PathScientistScanBotProfileEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextId { get; set; }
        public uint Creature2Id { get; set; }
        public uint ScanTimeMS { get; set; }
        public uint ProcessingTimeMS { get; set; }
        public float PlayerRadius { get; set; }
        public float ScanAOERange { get; set; }
        public float MaxSeekDistance { get; set; }
        public float SpeedMultiplier { get; set; }
        public float ThoroughnessMultiplier { get; set; }
        public float HealthMultiplier { get; set; }
        public float HealthRegenMultiplier { get; set; }
        public uint MinCooldownTimeMs { get; set; }
        public uint MaxCooldownTimeMs { get; set; }
        public float MaxCooldownDistance { get; set; }
        public uint PathScientistScanBotProfileFlags { get; set; }
        public uint SocketCount { get; set; }
    }
}
