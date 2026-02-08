namespace NexusForever.Network.Internal.Message.Match
{
    public class MatchMemberLeftMessage
    {
        public Shared.Match Match { get; set; }
        public Shared.MatchTeam Team { get; set; }
        public Shared.MatchTeamMember Member { get; set; }
    }
}
