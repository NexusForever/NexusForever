using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NexusForever.Shared.Configuration
{
    public static class Logging
    {
        public static ILoggerFactory Factory => DependencyInjection.ServiceProvider.GetRequiredService<ILoggerFactory>();

        public static ILogger GetLogger<T>()
        {
            return Factory.CreateLogger<T>();
        }
    }
    public static class SharedConfiguration
    {
        public static IConfiguration Configuration { get; private set; }

        public static void Initialize(string file)
        {
            if (Configuration != null) return;
            ConfigurationBuilder builder = new ConfigurationBuilder();

            builder
                .AddJsonFile(file, false, true)
                .AddEnvironmentVariables()
                .AddCommandLine(Environment.GetCommandLineArgs().Skip(1).ToArray());

            Configuration = builder.Build();
        }
    }
}