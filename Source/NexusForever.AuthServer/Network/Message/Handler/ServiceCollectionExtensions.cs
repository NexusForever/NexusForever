using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network;

namespace NexusForever.AuthServer.Network.Message.Handler
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAuthNetworkMessageHandler(this IServiceCollection sc)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetNetworkHandlerTypes())
                sc.AddTransient(type);
        }
    }
}
