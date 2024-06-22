using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network.Message.Handler;
using NexusForever.Network.Message.Model;

namespace NexusForever.Network.Message
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNetworkMessage(this IServiceCollection sc)
        {
            sc.AddNetworkMessageModel();
            sc.AddNetworkMessageHandler();
        }
    }
}
