using NexusForever.Game.Static.Reputation;

namespace NexusForever.Network.Internal.Message.Match.Shared
{
    public class MatchTeam
    {
        public Game.Static.Matching.MatchTeam Team { get; set; }
        public Faction Faction { get; set; }
        public List<MatchTeamMember> Members { get; set; }
    }
}
