namespace NexusForever.GameTable.Model
{
    public class CombatRewardEntry
    {
        public uint Id { get; set; }
        public uint CombatRewardTypeEnum { get; set; }
        public uint LocalizedTextIdCombatFloater { get; set; }
        public uint LocalizedTextIdCombatLogMessage { get; set; }
        public uint VisualEffectIdVisual00 { get; set; }
        public uint VisualEffectIdVisual01 { get; set; }
    }
}
