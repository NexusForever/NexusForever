using System;
using System.IO;
using System.Reflection;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Database;
using NexusForever.Shared.Network;
using NexusForever.StsServer.Network;
using NexusForever.StsServer.Network.Message;
using NLog;

namespace NexusForever.StsServer
{
    internal static class StsServer
    {
        #if DEBUG
        private const string Title = "NexusForever: STS Server (DEBUG)";
        #else
        private const string Title = "NexusForever: STS Server (RELEASE)";
        #endif

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static void Main()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            Console.Title = Title;
            log.Info("Initialising...");

            ConfigurationManager<StsServerConfiguration>.Initialise("StsServer.json");
            DatabaseManager.Initialise(ConfigurationManager<StsServerConfiguration>.Config.Database);
            
            MessageManager.Initialise();
            NetworkManager<StsSession>.Initialise(ConfigurationManager<StsServerConfiguration>.Config.Network);

            WorldManager.Initialise(lastTick =>
            {
                NetworkManager<StsSession>.Update(lastTick);
            });

            log.Info("Ready!");
        }
    }
}
