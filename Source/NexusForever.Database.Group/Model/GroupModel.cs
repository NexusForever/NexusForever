using NexusForever.Game.Static.Group;
using NexusForever.Game.Static.Matching;

namespace NexusForever.Database.Group.Model
{
    public class GroupModel
    {
        public ulong GroupId { get; set; }
        public GroupFlags Flags { get; set; }
        public LootRule LootRule { get; set; }
        public LootRule LootRuleThreshold { get; set; }
        public LootThreshold LootThreshold { get; set; }
        public HarvestLootRule LootRuleHarvest { get; set; }
        public Guid? Match { get; set; }
        public MatchTeam? MatchTeam { get; set; }

        public GroupLeaderModel Leader { get; set; }
        public GroupRequestModel Request { get; set; }
        public List<GroupMarkerModel> Markers { get; set; } = [];
        public List<GroupMemberModel> Members { get; set; } = [];
        public List<GroupInviteModel> Invites { get; set; } = [];
    }
}
