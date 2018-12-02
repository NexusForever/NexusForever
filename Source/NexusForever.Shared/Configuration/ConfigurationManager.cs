using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace NexusForever.Shared.Configuration
{
    public static class ConfigurationManager<T>
    {
        public static T Config { get; private set; }
        public static IConfiguration Configuration { get; private set; }

        public static void Initialise(string file)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();

            builder
                .AddJsonFile(file, false, true)
                .AddEnvironmentVariables()
                .AddCommandLine(Environment.GetCommandLineArgs().Skip(1).ToArray());

            Configuration = builder.Build();
            Config = Configuration.Get<T>();
        }
    }
}
