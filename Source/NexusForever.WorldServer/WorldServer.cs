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
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Game.RBAC;
using NexusForever.WorldServer.Game;
using NexusForever.WorldServer.Game.Achievement;
using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Movement;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Game.Prerequisite;
using NexusForever.WorldServer.Game.Quest;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Game.Storefront;
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

        public static ushort RealmId { get; private set; }

        private static void Main()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            Console.Title = Title;
            log.Info("Initialising...");

            ConfigurationManager<WorldServerConfiguration>.Instance.Initialise("WorldServer.json");

            DatabaseManager.Instance.Initialise(ConfigurationManager<WorldServerConfiguration>.Instance.Config.Database);
            DatabaseManager.Instance.Migrate();

            // RBACManager must be initialised before CommandManager
            RBACManager.Instance.Initialise();
            CommandManager.Instance.Initialise();

            DisableManager.Instance.Initialise();

            GameTableManager.Instance.Initialise();
            BaseMapManager.Instance.Initialise();
            SearchManager.Instance.Initialise();
            EntityManager.Instance.Initialise();
            EntityCommandManager.Instance.Initialise();
            EntityCacheManager.Instance.Initialise();
            GlobalMovementManager.Instance.Initialise();

            AssetManager.Instance.Initialise();
            PrerequisiteManager.Instance.Initialise();
            GlobalSpellManager.Instance.Initialise();
            GlobalQuestManager.Instance.Initialise();

            CharacterManager.Instance.Initialise();
            ResidenceManager.Instance.Initialise();
            GlobalStorefrontManager.Instance.Initialise();

            GlobalAchievementManager.Instance.Initialise();

            RealmId = ConfigurationManager<WorldServerConfiguration>.Instance.Config.RealmId;
            ServerManager.Instance.Initialise(RealmId); 

            MessageManager.Instance.Initialise();
            SocialManager.Instance.Initialise();
            NetworkManager<WorldSession>.Instance.Initialise(ConfigurationManager<WorldServerConfiguration>.Instance.Config.Network);
            WorldManager.Instance.Initialise(lastTick =>
            {
                // NetworkManager must be first and MapManager must come before everything else
                NetworkManager<WorldSession>.Instance.Update(lastTick);
                MapManager.Instance.Update(lastTick);

                ResidenceManager.Instance.Update(lastTick);
                BuybackManager.Instance.Update(lastTick);
                GlobalQuestManager.Instance.Update(lastTick);

                // process commands after everything else in the tick has processed
                CommandManager.Instance.Update(lastTick);
            });

            using (WorldServerEmbeddedWebServer.Initialise())
            {
                log.Info("Ready!");

                while (true)
                {
                    Console.Write(">> ");
                    string line = Console.ReadLine();
                    CommandManager.Instance.HandleCommandDelay(new ConsoleCommandContext(), line);
                }
            }
        }
    }
}
