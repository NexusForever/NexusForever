using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network;
using NexusForever.StsServer.Network.Message;

namespace NexusForever.StsServer.Network
{
    public static class ServiceCollectionExtensions
    {
        public static void AddStsNetwork(this IServiceCollection sc)
        {
            sc.AddSingleton<IMessageManager, MessageManager>();

            sc.AddNetworkConnectivity<IStsSession, StsSession>();
        }
    }
}
