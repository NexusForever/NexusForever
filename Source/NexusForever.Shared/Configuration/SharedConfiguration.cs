using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace NexusForever.Shared.Configuration
{
    public static class SharedConfiguration
    {
        public static IConfiguration Configuration { get; private set; }

        private static string fileLocation;

        public static void Initialise(string file)
        {
            if (Configuration != null)
                return;

            fileLocation = file;

            var builder = new ConfigurationBuilder()
                .AddJsonFile(file, false, true)
                .AddEnvironmentVariables()
                .AddCommandLine(Environment.GetCommandLineArgs().Skip(1).ToArray());

            Configuration = builder.Build();
        }

        public static bool Save<T>(object config)
        {
            try
            {
                if (fileLocation.Length > 0)
                    File.WriteAllText(fileLocation, JsonConvert.SerializeObject(config, Formatting.Indented));

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
