namespace NexusForever.Shared.GameTable.Model
{
    public class RewardTrackRewardsEntry
    {
        public uint Id;
        public uint RewardTrackId;
        public uint RewardPointFlags;
        public uint PrerequisiteId;
        public uint Flags;
        public uint CurrencyTypeId;
        public uint CurrencyAmount;
        [GameTableFieldArray(3u)]
        public uint[] RewardTrackRewardTypeEnums;
        [GameTableFieldArray(3u)]
        public uint[] RewardChoiceIds;
        [GameTableFieldArray(3u)]
        public uint[] RewardChoiceCounts;
    }
}
