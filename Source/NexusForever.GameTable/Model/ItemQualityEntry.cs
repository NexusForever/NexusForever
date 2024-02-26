namespace NexusForever.GameTable.Model
{
    public class ItemQualityEntry
    {
        public uint Id { get; set; }
        public float SalvageCritChance { get; set; }
        public float TurninMultiplier { get; set; }
        public float RuneCostMultiplier { get; set; }
        public float DyeCostMultiplier { get; set; }
        public uint VisualEffectIdLoot { get; set; }
        public float IconColorR { get; set; }
        public float IconColorG { get; set; }
        public float IconColorB { get; set; }
        public uint DefaultRunes { get; set; }
        public uint MaxRunes { get; set; }
        public string AssetPathDieModel { get; set; }
        public uint SoundEventIdFortuneCardFanfare { get; set; }
    }
}
