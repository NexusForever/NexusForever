using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Server.ChatServer.Chat
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddChat(this IServiceCollection sc)
        {
            sc.AddTransient<ChatChannel>();
            sc.AddTransient<ChatChannelMember>();

            sc.AddScoped<ChatChannelManager>();

            return sc;
        }
    }
}
