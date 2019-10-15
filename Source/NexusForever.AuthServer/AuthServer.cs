using System;
using System.IO;
using System.Reflection;
using NLog;
using NexusForever.AuthServer.Network;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Database;
using NexusForever.Shared.Game;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.AuthServer
{
    internal static class AuthServer
    {
        #if DEBUG
        private const string Title = "NexusForever: Authentication Server (DEBUG)";
        #else
        private const string Title = "NexusForever: Authentication Server (RELEASE)";
        #endif

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static void Main()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            Console.Title = Title;
            log.Info("Initialising...");

            ConfigurationManager<AuthServerConfiguration>.Initialise("AuthServer.json");
            DatabaseManager.Initialise(ConfigurationManager<AuthServerConfiguration>.Config.Database);

            ServerManager.Initialise();

            MessageManager.Initialise();
            NetworkManager<AuthSession>.Initialise(ConfigurationManager<AuthServerConfiguration>.Config.Network);

            WorldManager.Initialise(lastTick =>
            {
                NetworkManager<AuthSession>.Update(lastTick);
            });

            log.Info("Ready!");
        }
    }
}
