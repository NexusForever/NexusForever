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

            sc.AddSingleton<IMatchFactory, MatchFactory>();
            sc.AddSingleton<IFactoryInterface<IMatch>, FactoryInterface<IMatch>>();
            sc.AddTransient<IMatch, Match>();
            sc.AddTransient<IPvpMatch, PvpMatch>();

            sc.AddTransientFactory<IMatchTeam, MatchTeam>();
            sc.AddTransientFactory<IMatchTeamMember, MatchTeamMember>();

            sc.AddTransientFactory<IMatchCharacter, MatchCharacter>();
        }
    }
}
