using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Network.Message.Handler
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNetworkMessageHandler(this IServiceCollection sc)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetNetworkHandlerTypes())
                sc.AddTransient(type);
        }
    }
}
