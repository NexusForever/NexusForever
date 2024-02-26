namespace NexusForever.GameTable.Model
{
    public class HazardEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdTooltip { get; set; }
        public float MeterChangeRate { get; set; }
        public uint MeterMaxValue { get; set; }
        public uint Flags { get; set; }
        public uint HazardTypeEnum { get; set; }
        public uint Spell4IdDamage { get; set; }
        public float MinDistanceToUnit { get; set; }
        public float MeterThreshold00 { get; set; }
        public float MeterThreshold01 { get; set; }
        public float MeterThreshold02 { get; set; }
        public uint Spell4IdThresholdProc00 { get; set; }
        public uint Spell4IdThresholdProc01 { get; set; }
        public uint Spell4IdThresholdProc02 { get; set; }
    }
}
