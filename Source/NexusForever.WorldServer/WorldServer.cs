using System;
using System.IO;
using System.Reflection;
using NLog;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Database;
using NexusForever.Shared.Game;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Game;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer
{
    internal static class WorldServer
    {
        #if DEBUG
        private const string Title = "NexusForever: World Server (DEBUG)";
        #else
        private const string Title = "NexusForever: World Server (RELEASE)";
        #endif

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static void Main()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            Console.Title = Title;
            log.Info("Initialising...");

            ConfigurationManager<WorldServerConfiguration>.Initialise("WorldServer.json");

            DatabaseManager.Initialise(ConfigurationManager<WorldServerConfiguration>.Config.Database);

            GameTableManager.Initialise();

            EntityManager.Initialise();
            EntityCommandManager.Initialise();

            AssetManager.Initialise();
            ServerManager.Initialise();

            MessageManager.Initialise();
            CommandManager.Initialise();
            NetworkManager<WorldSession>.Initialise(ConfigurationManager<WorldServerConfiguration>.Config.Network);

            WorldManager.Initialise(lastTick =>
            {
                NetworkManager<WorldSession>.Update(lastTick);
                MapManager.Update(lastTick);
            });

            log.Info("Ready!");

            while (true)
            {
                Console.Write(">> ");
                string line = Console.ReadLine();
                CommandManager.ParseCommand(line, out string command, out string[] parameters);

                CommandHandlerDelegate handler = CommandManager.GetCommandHandler(command);
                if (handler != null)
                {
                    try
                    {
                        handler.Invoke(null, parameters);
                    }
                    catch (Exception exception)
                    {
                        log.Error(exception);
                    }
                }
                else
                    Console.WriteLine("Invalid command!");
            }
        }
    }
}
