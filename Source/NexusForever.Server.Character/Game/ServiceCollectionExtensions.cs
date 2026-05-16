using Microsoft.Extensions.DependencyInjection;
using NexusForever.Server.Character.Game.Character;
using NexusForever.Server.Character.Game.Who;

namespace NexusForever.Server.Character.Game
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGame(this IServiceCollection sc)
        {
            sc.AddScoped<CharacterManager>();
            sc.AddTransient<Character.Character>();

            sc.AddTransient<QueryExecutor>();
            sc.AddTransient<QueryBuilder>();
        }
    }
}
