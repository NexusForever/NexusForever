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

            ConfigurationManager<AuthServerConfiguration>.Instance.Initialise("AuthServer.json");

            DatabaseManager.Instance.Initialise(ConfigurationManager<AuthServerConfiguration>.Instance.Config.Database);

            ServerManager.Instance.Initialise();

            MessageManager.Instance.Initialise();
            NetworkManager<AuthSession>.Instance.Initialise(ConfigurationManager<AuthServerConfiguration>.Instance.Config.Network);

            WorldManager.Instance.Initialise(lastTick =>
            {
                NetworkManager<AuthSession>.Instance.Update(lastTick);
            });

            log.Info("Ready!");
        }
    }
}
