using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network.Internal.Configuration;
using NexusForever.Network.Internal.Static;
using NexusForever.Network.Internal.Telemetry.Trace;
using Rebus.Config;
using Rebus.Pipeline;
using Rebus.Pipeline.Receive;
using Rebus.Pipeline.Send;

namespace NexusForever.Network.Internal
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetworkInternalBroker(this IServiceCollection sc, BrokerConfig config)
        {
            sc.AddRebus(b => b
                .Transport(t =>
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
                }).Options(o =>
                {
                    o.Decorate<IPipeline>(c =>
                    {
                        var pipeline = c.Get<IPipeline>();
                        var outgoingStep = new InjectTraceContextOutgoingStep();
                        var incomingStep = new ExtractTraceContextIncomingStep();
                        return new PipelineStepInjector(pipeline)
                            .OnSend(outgoingStep, PipelineRelativePosition.Before, typeof(SerializeOutgoingMessageStep))
                            .OnReceive(incomingStep, PipelineRelativePosition.After, typeof(DeserializeIncomingMessageStep));
                    });
                }));

            sc.AddTransient<IInternalMessagePublisher, RebusMessagePublisher>();

            return sc;
        }
    }
}
