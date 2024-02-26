namespace NexusForever.GameTable.Model
{
    public class Spell4ThresholdsEntry
    {
        public uint Id { get; set; }
        public uint Spell4IdParent { get; set; }
        public uint Spell4IdToCast { get; set; }
        public uint OrderIndex { get; set; }
        public uint ThresholdDuration { get; set; }
        public uint VitalEnumCostType00 { get; set; }
        public uint VitalEnumCostType01 { get; set; }
        public uint VitalCostValue00 { get; set; }
        public uint VitalCostValue01 { get; set; }
        public uint LocalizedTextIdTooltip { get; set; }
        public string IconReplacement { get; set; }
        public uint VisualEffectId { get; set; }
    }
}
