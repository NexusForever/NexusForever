using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Shared;

namespace NexusForever.Game.Matching.Match
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameMatchingMatch(this IServiceCollection sc)
        {
            sc.AddSingleton<IMatchManager, MatchManager>();

            sc.AddTransientFactory<IMatchProposal, MatchProposal>();
            sc.AddTransientFactory<IMatchProposalTeam, MatchProposalTeam>();
            sc.AddTransientFactory<IMatchProposalTeamMember, MatchProposalTeamMember>();

            sc.AddTransientFactory<IMatch, Match>();
            sc.AddTransientFactory<IMatchTeam, MatchTeam>();
            sc.AddTransientFactory<IMatchTeamMember, MatchTeamMember>();
        }
    }
}
