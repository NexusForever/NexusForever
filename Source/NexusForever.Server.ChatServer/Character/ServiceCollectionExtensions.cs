using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Server.ChatServer.Character
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCharacter(this IServiceCollection sc)
        {
            sc.AddTransient<Character>();
            sc.AddTransient<CharacterChatChannel>();
            
            sc.AddScoped<CharacterManager>();

            return sc;
        }
    }
}
