namespace NexusForever.GameTable.Model
{
    public class RewardTrackEntry
    {
        public uint Id { get; set; }
        public uint RewardTrackTypeEnum { get; set; }
        public uint PrerequisiteId { get; set; }
        public uint LocalizedTextId { get; set; }
        public uint LocalizedTextIdSummary { get; set; }
        public string AssetPathImage { get; set; }
        public string AssetPathButtonImage { get; set; }
        public uint RewardPointCost00 { get; set; }
        public uint RewardPointCost01 { get; set; }
        public uint RewardPointCost02 { get; set; }
        public uint RewardPointCost03 { get; set; }
        public uint RewardPointCost04 { get; set; }
        public uint RewardPointCost05 { get; set; }
        public uint RewardPointCost06 { get; set; }
        public uint RewardPointCost07 { get; set; }
        public uint RewardPointCost08 { get; set; }
        public uint RewardPointCost09 { get; set; }
        public uint RewardTrackIdParent { get; set; }
        public uint Flags { get; set; }
    }
}
