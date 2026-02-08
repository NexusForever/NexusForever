using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Server.GroupServer.Character
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCharacter(this IServiceCollection sc)
        {
            sc.AddScoped<CharacterManager>();
            sc.AddTransient<Character>();
            sc.AddTransient<CharacterGroup>();
            return sc;
        }
    }
}
