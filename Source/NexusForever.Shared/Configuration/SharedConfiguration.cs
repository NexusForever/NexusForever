using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace NexusForever.Shared.Configuration
{
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