namespace NexusForever.GameTable.Model
{
    public class FinishingMoveDeathVisualEntry
    {
        public uint Id { get; set; }
        public uint Priority { get; set; }
        public uint DamageTypeFlags { get; set; }
        public uint Creature2MinSize { get; set; }
        public uint Creature2MaxSize { get; set; }
        public uint CreatureMaterialEnum { get; set; }
        public uint MovementStateFlags { get; set; }
        public string DeathModelAsset { get; set; }
        public uint ModelSequenceIdDeath { get; set; }
        public uint VisualEffectIdDeath00 { get; set; }
        public uint VisualEffectIdDeath01 { get; set; }
        public uint VisualEffectIdDeath02 { get; set; }
    }
}
