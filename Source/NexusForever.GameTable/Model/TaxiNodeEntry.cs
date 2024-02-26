namespace NexusForever.GameTable.Model
{
    public class TaxiNodeEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextId { get; set; }
        public uint TaxiNodeTypeEnum { get; set; }
        public uint Flags { get; set; }
        public uint FlightPathTypeEnum { get; set; }
        public uint TaxiNodeFactionEnum { get; set; }
        public uint WorldLocation2Id { get; set; }
        public uint ContentTier { get; set; }
        public uint AutoUnlockLevel { get; set; }
        public uint RecommendedMinLevel { get; set; }
        public uint RecommendedMaxLevel { get; set; }
    }
}
