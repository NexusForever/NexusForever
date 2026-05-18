using NexusForever.Game.Abstract.Matching.Match;
using InternalMatch = NexusForever.Network.Internal.Message.Match.Shared.Match;
using InternalMatchTeam = NexusForever.Network.Internal.Message.Match.Shared.MatchTeam;
using InternalMatchTeamMember = NexusForever.Network.Internal.Message.Match.Shared.MatchTeamMember;

namespace NexusForever.Game.Matching.Match
{
    public static class MatchMappingExtensions
    {
        public static InternalMatch ToInternalMatch(this IMatch match)
        {
            return new InternalMatch
            {
                Guid            = match.Guid,
                GameMapEntryId  = match.MatchingMap.GameMapEntry.Id,
                GameTypeEntryId = match.MatchingMap.GameTypeEntry.Id,
                Teams           = match.GetTeams().Select(t => t.ToInternalMatchTeam()).ToList()
            };
        }

        public static InternalMatchTeam ToInternalMatchTeam(this IMatchTeam team)
        {
            return new InternalMatchTeam
            {
                Team    = team.Team,
                Faction = team.Faction,
                Members = team.GetMembers().Select(m => m.ToInternalMatchTeamMember()).ToList()
            };
        }

        public static InternalMatchTeamMember ToInternalMatchTeamMember(this IMatchTeamMember member)
        {
            return new InternalMatchTeamMember
            {
                Identity = member.Identity.ToInternalIdentity(),
                Roles    = member.Roles
            };
        }
    }
}
