using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.GameTable.Configuration.Model;
using NexusForever.GameTable.Text;
using NexusForever.Shared;

namespace NexusForever.GameTable
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameTable(this IServiceCollection sc, IConfigurationSection configuration)
        {
            sc.AddGameTableText();

            sc.AddOptions<GameTableConfig>()
                .Bind(configuration)
                .ValidateOnStart();

            sc.AddSingletonLegacy<IGameTableManager, GameTableManager>();
        }

        public static void AddGameTable(this IServiceCollection sc, Action<GameTableConfig> configuration)
        {
            sc.AddGameTableText();

            sc.AddOptions<GameTableConfig>()
                .Configure(configuration)
                .ValidateOnStart();

            sc.AddSingletonLegacy<IGameTableManager, GameTableManager>();
        }
    }
}
