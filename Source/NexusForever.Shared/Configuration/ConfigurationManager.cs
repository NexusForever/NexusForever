using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace NexusForever.Shared.Configuration
{
    public static class ConfigurationManager<T>
    {
        public static T Config { get; private set; }
        public static IConfiguration Configuration { get; private set; }

        public static void Initialise(string file)
        {
            string fileContents = File.ReadAllText(file);
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile(file, false, true).AddEnvironmentVariables();
            Configuration = builder.Build();
            Config = JsonConvert.DeserializeObject<T>(fileContents);
        }
    }
}
