using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Database;
using NexusForever.Shared.Game;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Game;
using NexusForever.WorldServer.Game.Achievement;
using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Movement;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Guild;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Game.Prerequisite;
using NexusForever.WorldServer.Game.Quest;
using NexusForever.WorldServer.Game.RBAC;
using NexusForever.WorldServer.Game.Reputation;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Game.Storefront;
using NexusForever.WorldServer.Game.TextFilter;
using NexusForever.WorldServer.Network;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

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

        /// <summary>
        /// Internal unique id of the realm.
        /// </summary>
        public static ushort RealmId { get; private set; }
        private static volatile bool shutdownRequested;
        private static Thread worldThread;

        /// <summary>
        /// Realm message of the day that is shown to players on login.
        /// </summary>
        public static string RealmMotd { get; set; }

        private static void Main()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location));

            Console.Title = Title;
            log.Info("Initialising...");

            var managersList = new List<IShutdown>();

            managersList.Add(ConfigurationManager<WorldServerConfiguration>.Instance.Initialise("WorldServer.json"));
            RealmId   = ConfigurationManager<WorldServerConfiguration>.Instance.Config.RealmId;
            RealmMotd = ConfigurationManager<WorldServerConfiguration>.Instance.Config.MessageOfTheDay;

            managersList.Add(DatabaseManager.Instance.Initialise(ConfigurationManager<WorldServerConfiguration>.Instance.Config.Database));
            DatabaseManager.Instance.Migrate();

            // RBACManager must be initialised before CommandManager
            managersList.Add(RBACManager.Instance.Initialise());
            managersList.Add(CommandManager.Instance.Initialise());

            managersList.Add(DisableManager.Instance.Initialise());
            managersList.Add(GameTableManager.Instance.Initialise());
            managersList.Add(BaseMapManager.Instance.Initialise());
            managersList.Add(SearchManager.Instance.Initialise());
            managersList.Add(EntityManager.Instance.Initialise());
            managersList.Add(EntityCommandManager.Instance.Initialise());
            managersList.Add(EntityCacheManager.Instance.Initialise());
            managersList.Add(FactionManager.Instance.Initialise());
            managersList.Add(GlobalMovementManager.Instance.Initialise());

            managersList.Add(GlobalGuildManager.Instance.Initialise());
            managersList.Add(AssetManager.Instance.Initialise());
            managersList.Add(PrerequisiteManager.Instance.Initialise());
            managersList.Add(GlobalSpellManager.Instance.Initialise());
            managersList.Add(GlobalQuestManager.Instance.Initialise());

            managersList.Add(CharacterManager.Instance.Initialise());
            managersList.Add(ResidenceManager.Instance.Initialise());
            managersList.Add(GlobalStorefrontManager.Instance.Initialise());

            managersList.Add(GlobalAchievementManager.Instance.Initialise());
            managersList.Add(ServerManager.Instance.Initialise(RealmId));

            managersList.Add(MessageManager.Instance.Initialise());
            managersList.Add(SocialManager.Instance.Initialise());
            managersList.Add(NetworkManager<WorldSession>.Instance.Initialise(ConfigurationManager<WorldServerConfiguration>.Instance.Config.Network));
            managersList.Add(TextFilterManager.Instance.Initialise());
            WorldManager.Instance.Initialise(lastTick =>
            {
                // NetworkManager must be first and MapManager must come before everything else
                NetworkManager<WorldSession>.Instance.Update(lastTick);
                MapManager.Instance.Update(lastTick);

                ResidenceManager.Instance.Update(lastTick);
                BuybackManager.Instance.Update(lastTick);
                GlobalQuestManager.Instance.Update(lastTick);
                GlobalGuildManager.Instance.Update(lastTick);

                // process commands after everything else in the tick has processed
                CommandManager.Instance.Update(lastTick);
            }, managersList);

            WorldManager.Instance.OnShutdown += OnShutdown;
            managersList.Add(WorldServerEmbeddedWebServer.Instance.Initialise());
            log.Info("Ready!");

            worldThread = new Thread(() => 
            {
                while (!shutdownRequested)
                {
                    Console.Write(">> ");
                    string line = Console.ReadLine();
                    if (!shutdownRequested)
                    {
                        CommandManager.Instance.HandleCommandDelay(new ConsoleCommandContext(), line);
                    }
                }
            });
            worldThread.IsBackground = true;
            worldThread.Start();
        }

        private static void OnShutdown()
        {
            shutdownRequested = true;
        }
    }
}
