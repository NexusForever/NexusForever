using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Event;
using NexusForever.Shared;

namespace NexusForever.Game.Event
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameEvent(this IServiceCollection sc)
        {
            sc.AddSingleton<IPublicEventTemplateManager, PublicEventTemplateManager>();
            sc.AddTransientFactory<IPublicEventTemplate, PublicEventTemplate>();

            sc.AddTransient<IPublicEventFactory, PublicEventFactory>();
            sc.AddTransientFactory<IPublicEvent, PublicEvent>();
            sc.AddTransientFactory<IPublicEventTeam, PublicEventTeam>();
            sc.AddTransientFactory<IPublicEventObjective, PublicEventObjective>();
            sc.AddTransientFactory<IPublicEventTeamMember, PublicEventTeamMember>();

            sc.AddTransient<IPublicEventEntityFactory, PublicEventEntityFactory>();

            sc.AddTransient<IPublicEventStats, PublicEventStats>();

            sc.AddTransientFactory<IPublicEventCharacter, PublicEventCharacter>();

            sc.AddTransientFactory<IPublicEventVote, PublicEventVote>();
            sc.AddTransientFactory<IPublicEventVoteResponse, PublicEventVoteResponse>();
        }
    }
}
