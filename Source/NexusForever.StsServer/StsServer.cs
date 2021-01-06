using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Database;
using NexusForever.Shared.Network;
using NexusForever.StsServer.Network;
using NexusForever.StsServer.Network.Message;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NexusForever.StsServer
{
    internal static class StsServer
    {
        #if DEBUG
        private const string Title = "NexusForever: STS Server (DEBUG)";
        #else
        private const string Title = "NexusForever: STS Server (RELEASE)";
        #endif

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private static void Main()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location));

            Console.Title = Title;
            Log.Info("Initialising...");

            List<IShutdown> managersList = new List<IShutdown>
            {
                ConfigurationManager<StsServerConfiguration>.Instance.Initialise("StsServer.json"),
                DatabaseManager.Instance.Initialise(ConfigurationManager<StsServerConfiguration>.Instance.Config.Database),
                MessageManager.Instance.Initialise(),
                NetworkManager<StsSession>.Instance.Initialise(ConfigurationManager<StsServerConfiguration>.Instance.Config.Network)
            };

            WorldManager.Instance.Initialise(lastTick =>
            {
                NetworkManager<StsSession>.Instance.Update(lastTick);
            }, managersList);

            Log.Info("Ready!");
        }
    }
}
