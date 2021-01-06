using NexusForever.AuthServer.Network;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Database;
using NexusForever.Shared.Game;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location));

            Console.Title = Title;
            log.Info("Initialising...");

            List<IShutdown> managersList = new List<IShutdown>();

            managersList.Add(ConfigurationManager<AuthServerConfiguration>.Instance.Initialise("AuthServer.json"));

            managersList.Add(DatabaseManager.Instance.Initialise(ConfigurationManager<AuthServerConfiguration>.Instance.Config.Database));

            managersList.Add(ServerManager.Instance.Initialise());

            managersList.Add(MessageManager.Instance.Initialise());
            managersList.Add(NetworkManager<AuthSession>.Instance.Initialise(ConfigurationManager<AuthServerConfiguration>.Instance.Config.Network));

            WorldManager.Instance.Initialise(lastTick =>
            {
                NetworkManager<AuthSession>.Instance.Update(lastTick);
            }, managersList);

            log.Info("Ready!");
        }
    }
}
