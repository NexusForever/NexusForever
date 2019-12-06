namespace NexusForever.Shared.GameTable.Model
{
    public class RewardTrackEntry
    {
        public uint Id;
        public uint RewardTrackTypeEnum;
        public uint PrerequisiteId;
        public uint LocalizedTextId;
        public uint LocalizedTextIdSummary;
        public string AssetPathImage;
        public string AssetPathButtonImage;
        [GameTableFieldArray(10u)]
        public uint[] RewardPointCosts;
        public uint RewardTrackIdParent;
        public uint Flags;
    }
}
