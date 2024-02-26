namespace NexusForever.GameTable.Model
{
    public class TradeskillSchematic2Entry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint TradeSkillId { get; set; }
        public uint Item2IdOutput { get; set; }
        public uint Item2IdOutputFail { get; set; }
        public uint OutputCount { get; set; }
        public uint LootId { get; set; }
        public uint Tier { get; set; }
        public uint Flags { get; set; }
        public uint Item2IdMaterial00 { get; set; }
        public uint Item2IdMaterial01 { get; set; }
        public uint Item2IdMaterial02 { get; set; }
        public uint Item2IdMaterial03 { get; set; }
        public uint Item2IdMaterial04 { get; set; }
        public uint MaterialCost00 { get; set; }
        public uint MaterialCost01 { get; set; }
        public uint MaterialCost02 { get; set; }
        public uint MaterialCost03 { get; set; }
        public uint MaterialCost04 { get; set; }
        public uint TradeskillSchematic2IdParent { get; set; }
        public float VectorX { get; set; }
        public float VectorY { get; set; }
        public float Radius { get; set; }
        public float CritRadius { get; set; }
        public uint Item2IdOutputCrit { get; set; }
        public uint OutputCountCritBonus { get; set; }
        public uint Priority { get; set; }
        public uint MaxAdditives { get; set; }
        public uint DiscoverableQuadrant { get; set; }
        public float DiscoverableRadius { get; set; }
        public float DiscoverableAngle { get; set; }
        public uint TradeskillCatalystOrderingId { get; set; }
    }
}
