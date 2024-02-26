namespace NexusForever.GameTable.Model
{
    public class EldanAugmentationEntry
    {
        public uint Id { get; set; }
        public uint DisplayRow { get; set; }
        public uint DisplayColumn { get; set; }
        public uint ClassId { get; set; }
        public uint PowerCost { get; set; }
        public uint EldanAugmentationIdRequired { get; set; }
        public uint Spell4IdAugment { get; set; }
        public uint Item2IdUnlock { get; set; }
        public uint EldanAugmentationCategoryId { get; set; }
        public uint CategoryTier { get; set; }
        public uint LocalizedTextIdTitle { get; set; }
        public uint LocalizedTextIdTooltip { get; set; }
    }
}
