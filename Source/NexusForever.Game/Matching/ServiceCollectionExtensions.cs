using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Matching.Match;
using NexusForever.Game.Matching.Queue;
using NexusForever.Shared;

namespace NexusForever.Game.Matching
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameMatching(this IServiceCollection sc)
        {
            sc.AddGameMatchingMatch();

            sc.AddSingleton<IMatchingDataManager, MatchingDataManager>();
            sc.AddSingleton<IMatchingManager, MatchingManager>();

            sc.AddTransientFactory<IMatchingCharacter, MatchingCharacter>();
            
            sc.AddTransientFactory<IMatchingQueueManager, MatchingQueueManager>();
            sc.AddTransientFactory<IMatchingQueue, MatchingQueue>();
            sc.AddTransientFactory<IMatchingQueueValidator, MatchingQueueValidator>();
            sc.AddTransient<IMatchingQueueTimeManager, MatchingQueueTimeManager>();
            sc.AddTransient<IMatchingQueueMatcher, MatchingQueueMatcher>();
            sc.AddTransient<IMatchingQueueGroupMatcher, MatchingQueueGroupMatcher>();

            sc.AddTransientFactory<IMatchingQueueProposal, MatchingQueueProposal>();
            sc.AddTransientFactory<IMatchingQueueProposalMember, MatchingQueueProposalMember>();

            sc.AddTransientFactory<IMatchingQueueGroup, MatchingQueueGroup>();
            sc.AddTransientFactory<IMatchingQueueGroupTeam, MatchingQueueGroupTeam>();

            sc.AddTransientFactory<IMatchingRoleCheck, MatchingRoleCheck>();
            sc.AddTransientFactory<IMatchingRoleCheckMember, MatchingRoleCheckMember>();

            sc.AddTransient<IMatchingRoleEnforcer, MatchingRoleEnforcer>();
            sc.AddTransientFactory<IMatchingRoleEnforcerResult, MatchingRoleEnforcerResult>();

            sc.AddTransient<IMatchingMapSelector, MatchingMapSelector>();

            sc.AddSingleton<IMatchingQueueTimeManager, MatchingQueueTimeManager>();
            sc.AddTransientFactory<IMatchingQueueTime, MatchingQueueTime>();
        }
    }
}
