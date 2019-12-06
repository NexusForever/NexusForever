using System.Collections.Generic;

namespace NexusForever.Database.Character.Model
{
    public class CharacterRewardTrackModel
    {
        public ulong Id { get; set; }
        public uint RewardTrackId { get; set; }
        public ulong Points { get; set; }

        public CharacterModel Character { get; set; }
        public ICollection<CharacterRewardTrackMilestoneModel> Milestone { get; set; } = new HashSet<CharacterRewardTrackMilestoneModel>();
    }
}
