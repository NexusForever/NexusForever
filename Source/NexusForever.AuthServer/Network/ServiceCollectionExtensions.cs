using Microsoft.Extensions.DependencyInjection;
using NexusForever.AuthServer.Network.Message.Handler;
using NexusForever.Network;
using NexusForever.Network.Auth;

namespace NexusForever.AuthServer.Network
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAuthNetwork(this IServiceCollection sc)
        {
            sc.AddNetwork();
            sc.AddNetworkConnectivity<IAuthSession, AuthSession>();

            sc.AddNetworkAuth();
            sc.AddAuthNetworkMessageHandler();
        }
    }
}
