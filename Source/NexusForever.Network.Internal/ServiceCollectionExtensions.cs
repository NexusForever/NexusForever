using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network.Internal.Configuration;
using NexusForever.Network.Internal.Static;
using Rebus.Config;

namespace NexusForever.Network.Internal
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetworkInternal(this IServiceCollection sc)
        {
            sc.AddTransient<IInternalMessagePublisher, RebusMessagePublisher>();
            return sc;
        }

        public static IServiceCollection AddNetworkInternalBroker(this IServiceCollection sc, BrokerConfig config)
        {
            sc.AddRebus(b => b.Transport(t =>
            {
                switch (config.Broker)
                {
                    /*case BrokerProvider.InMemory:
                    {
                        t.UseInMemoryTransport()
                    }*/
                    case BrokerProvider.RabbitMQ:
                        t.UseRabbitMq(config.ConnectionString, config.InputQueue);
                        break;
                    case BrokerProvider.AzureServiceBus:
                        t.UseAzureServiceBus(config.ConnectionString, config.InputQueue);
                        break;
                }
            }));

            return sc;
        }
    }
}
