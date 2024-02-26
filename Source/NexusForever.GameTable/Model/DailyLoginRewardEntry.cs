namespace NexusForever.GameTable.Model
{
    public class DailyLoginRewardEntry
    {
        public uint Id { get; set; }
        public uint LoginDay { get; set; }
        public uint DailyLoginRewardTypeEnum { get; set; }
        public uint RewardObjectValue { get; set; }
        public uint DailyLoginRewardTierEnum { get; set; }
    }
}
