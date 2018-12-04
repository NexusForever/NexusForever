using System;
using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Shared.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            if (ServiceProvider != null)
                throw new InvalidOperationException("The service provider has already been set.");
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public static void Initialize(IServiceCollection serviceCollection)
        {
            Initialize(serviceCollection?.BuildServiceProvider());
        }

        public static void Initialize(Action<IServiceCollection> callback)
        {
            var serviceCollection = new ServiceCollection();
            callback(serviceCollection);
            Initialize(serviceCollection);
        }
    }
}
