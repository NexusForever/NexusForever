using System.Net;
using NexusForever.Database;
using NexusForever.Network.Internal.Static;

namespace NexusForever.Aspire.AppHost
{
    public static class ResourceBuilderExtensions
    {
        public static IResourceBuilder<T> WithNexusForeverTcp<T>(this IResourceBuilder<T> builder, IPAddress address, int port) where T : IResourceWithEnvironment, IResourceWithEndpoints
        {
            builder.WithEndpoint(port: port, targetPort: port, name: "tcp", isProxied: false);
            builder.WithEnvironment($"Network:Host", address.ToString());
            builder.WithEnvironment($"Network:Port", port.ToString());
            return builder;
        }

        public static IResourceBuilder<T> WithNexusForeverHttp<T>(this IResourceBuilder<T> builder, int port) where T : IResourceWithEnvironment, IResourceWithEndpoints
        {
            builder.WithHttpEndpoint(targetPort: port);

            builder.WithEnvironment(c =>
            {
                if (!c.Resource.TryGetEndpoints(out var endpoints))
                    return;

                foreach (EndpointAnnotation endpoint in endpoints)
                {
                    if (endpoint.Name != "http")
                        continue;

                    c.EnvironmentVariables["urls"] = $"{endpoint.UriScheme}://{endpoint.TargetHost}:{endpoint.TargetPort}";
                }
            });

            return builder;
        }

        public static IResourceBuilder<T> WithNexusForeverApi<T>(this IResourceBuilder<T> builder, string api, IResourceWithServiceDiscovery resource) where T : IResourceWithEnvironment
        {
            builder.WithReferenceRelationship(resource);

            builder.WithEnvironment(c =>
            {
                if (!resource.TryGetEndpoints(out var endpoints))
                    return;

                foreach (EndpointAnnotation endpoint in endpoints)
                {
                    if (endpoint.Name != "http")
                        continue;

                    if (endpoint.AllocatedEndpoint != null)
                        c.EnvironmentVariables[$"API:{api}:Host"] = endpoint.AllocatedEndpoint.UriString;
                    else
                        c.EnvironmentVariables[$"API:{api}:Host"] = $"{endpoint.UriScheme}://{endpoint.TargetHost}:{endpoint.TargetPort}";
                }
            });
            return builder;
        }

        public static IResourceBuilder<T> WithNexusForeverDatabase<T>(this IResourceBuilder<T> builder, string database, DatabaseProvider databaseProvider, IResourceWithConnectionString resource) where T : IResourceWithEnvironment
        {
            builder.WithReferenceRelationship(resource);

            builder.WithEnvironment($"Database:{database}:Provider", databaseProvider.ToString());
            builder.WithEnvironment(c =>
            {
                c.EnvironmentVariables[$"Database:{database}:ConnectionString"] = new ConnectionStringReference(resource, false);
            });
            return builder;
        }

        public static IResourceBuilder<T> WithNexusForeverMessageBroker<T>(this IResourceBuilder<T> builder, string inputQueue, BrokerProvider brokerProvider, IResourceWithConnectionString resource) where T : IResourceWithEnvironment
        {
            builder.WithEnvironment("Network:Internal:InputQueue", inputQueue);
            builder.WithEnvironment("Network:Internal:Broker", brokerProvider.ToString());
            builder.WithEnvironment(c =>
            {
                c.EnvironmentVariables["Network:Internal:ConnectionString"] = new ConnectionStringReference(resource, false);
            });
            return builder;
        }
    }
}
