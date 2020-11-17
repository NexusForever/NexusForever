namespace NexusForever.Database.Auth.Model
{
    public class AccountRewardTrackMilestoneModel
    {
        public uint Id { get; set; }
        public uint RewardTrackId { get; set; }
        public uint MilestoneId { get; set; }
        public uint PointsRequired { get; set; }
        public int Choice { get; set; }

        public AccountRewardTrackModel RewardTrack { get; set; }
    }
}
