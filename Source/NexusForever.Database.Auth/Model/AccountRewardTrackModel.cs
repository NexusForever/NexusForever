using System.Collections.Generic;

namespace NexusForever.Database.Auth.Model
{
    public class AccountRewardTrackModel
    {
        public uint Id { get; set; }
        public uint RewardTrackId { get; set; }
        public ulong Points { get; set; }

        public AccountModel Account { get; set; }
        public ICollection<AccountRewardTrackMilestoneModel> Milestone { get; set; } = new HashSet<AccountRewardTrackMilestoneModel>();
    }
}
