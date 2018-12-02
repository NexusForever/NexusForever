using System;
using System.IO;
using System.Linq;
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
            string[] arguments = Environment.GetCommandLineArgs();
            //string fileContents = File.ReadAllText(file);
            var builder = new ConfigurationBuilder();
            builder
                .AddJsonFile(file, false, true)
                .AddEnvironmentVariables()
                .AddCommandLine(Environment.GetCommandLineArgs().Skip(1).ToArray());
            
            Configuration = builder.Build();
            Config = Configuration.Get<T>();
            //Config = JsonConvert.DeserializeObject<T>(fileContents);
        }
    }
}
