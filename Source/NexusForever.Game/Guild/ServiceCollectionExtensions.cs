using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Static.Guild;
using NexusForever.Shared;

namespace NexusForever.Game.Guild
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameGuild(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IGlobalGuildManager, GlobalGuildManager>();

            sc.AddTransient<IGuildFactory, GuildFactory>();
            sc.AddKeyedTransient<IGuildBase, Guild>(GuildType.Guild);
            sc.AddKeyedTransient<IGuildBase, Circle>(GuildType.Circle);
            sc.AddKeyedTransient<IGuildBase, ArenaTeam>(GuildType.ArenaTeam2v2);
            sc.AddKeyedTransient<IGuildBase, ArenaTeam>(GuildType.ArenaTeam3v3);
            sc.AddKeyedTransient<IGuildBase, ArenaTeam>(GuildType.ArenaTeam5v5);
            sc.AddKeyedTransient<IGuildBase, WarParty>(GuildType.WarParty);
            sc.AddKeyedTransient<IGuildBase, Community>(GuildType.Community);
        }
    }
}
