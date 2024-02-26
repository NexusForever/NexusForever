namespace NexusForever.GameTable.Model
{
    public class RewardTrackRewardsEntry
    {
        public uint Id { get; set; }
        public uint RewardTrackId { get; set; }
        public uint RewardPointFlags { get; set; }
        public uint PrerequisiteId { get; set; }
        public uint Flags { get; set; }
        public uint CurrencyTypeId { get; set; }
        public uint CurrencyAmount { get; set; }
        public uint RewardTrackRewardTypeEnum00 { get; set; }
        public uint RewardTrackRewardTypeEnum01 { get; set; }
        public uint RewardTrackRewardTypeEnum02 { get; set; }
        public uint RewardChoiceId00 { get; set; }
        public uint RewardChoiceId01 { get; set; }
        public uint RewardChoiceId02 { get; set; }
        public uint RewardChoiceCount00 { get; set; }
        public uint RewardChoiceCount01 { get; set; }
        public uint RewardChoiceCount02 { get; set; }
    }
}
