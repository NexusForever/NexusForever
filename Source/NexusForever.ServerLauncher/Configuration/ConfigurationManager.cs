using Newtonsoft.Json;
using System.IO;

namespace NexusForever.ServerLauncher.Configuration
{
    public static class ConfigurationManager<T>
    {
        public static T Config { get; private set; }

        public static void Initialise(string file)
        {
            string fileContents = File.ReadAllText(file);
            Config = JsonConvert.DeserializeObject<T>(fileContents);
        }
    }
}
