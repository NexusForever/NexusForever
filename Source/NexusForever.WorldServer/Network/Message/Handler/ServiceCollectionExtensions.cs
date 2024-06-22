using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWorldNetworkMessageHandler(this IServiceCollection sc)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetNetworkHandlerTypes())
                sc.AddTransient(type);
        }
    }
}
