namespace NexusForever.Database.Character.Model
{
    public class CharacterRewardTrackMilestoneModel
    {
        public ulong Id { get; set; }
        public uint RewardTrackId { get; set; }
        public uint MilestoneId { get; set; }
        public uint PointsRequired { get; set; }
        public int Choice { get; set; }

        public CharacterRewardTrackModel RewardTrack { get; set; }
    }
}
