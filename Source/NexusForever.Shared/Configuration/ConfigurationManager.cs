using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace NexusForever.Shared.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceProvider ServiceProvider {get; private set;}

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
    public static class ConfigurationManager<T>
    {
        public static T Config { get; private set; }
        public static IConfiguration Configuration { get; private set; }
        public static void Initialise(string file)
        {
            string fileContents = File.ReadAllText(file);
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile(file, optional: false, reloadOnChange: true).AddEnvironmentVariables();
            Configuration = builder.Build();
            Config = JsonConvert.DeserializeObject<T>(fileContents);
        }
    }
}
