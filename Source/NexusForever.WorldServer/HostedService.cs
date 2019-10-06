using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NexusForever.Database;
using NexusForever.Database.Configuration.Model;
using NexusForever.Game;
using NexusForever.Game.Achievement;
using NexusForever.Game.Character;
using NexusForever.Game.Cinematic;
using NexusForever.Game.Combat;
using NexusForever.Game.Customisation;
using NexusForever.Game.Entity;
using NexusForever.Game.Entity.Movement;
using NexusForever.Game.Guild;
using NexusForever.Game.Housing;
using NexusForever.Game.Map;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Quest;
using NexusForever.Game.RBAC;
using NexusForever.Game.Reputation;
using NexusForever.Game.Server;
using NexusForever.Game.Social;
using NexusForever.Game.Spell;
using NexusForever.Game.Storefront;
using NexusForever.Game.Text.Filter;
using NexusForever.Game.Text.Search;
using NexusForever.GameTable;
using NexusForever.Network;
using NexusForever.Network.Configuration.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Social;
using NexusForever.Script;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Handler;

namespace NexusForever.WorldServer
{
    public class HostedService : IHostedService
    {
        private readonly ILogger log;

        private readonly IScriptManager scriptManager;
        private readonly IWorldManager worldManager;

        public HostedService(
            IServiceProvider serviceProvider,
            ILogger<IHostedService> log,
            IScriptManager scriptManager,
            IWorldManager worldManager)
        {
            LegacyServiceProvider.Provider = serviceProvider;

            this.log           = log;

            this.scriptManager = scriptManager;
            this.worldManager  = worldManager;
        }

        /// <summary>
        /// Start <see cref="WorldServer"/> and any related resources.
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            log.LogInformation("Starting...");

            SharedConfiguration.Instance.Initialise<WorldServerConfiguration>();

            RealmContext.Instance.Initialise();

            DatabaseManager.Instance.Initialise(SharedConfiguration.Instance.Get<DatabaseConfig>());
            DatabaseManager.Instance.Migrate();

            // RBACManager must be initialised before CommandManager
            RBACManager.Instance.Initialise();

            DisableManager.Instance.Initialise();

            scriptManager.Initialise();

            GameTableManager.Instance.Initialise();
            MapIOManager.Instance.Initialise();
            SearchManager.Instance.Initialise();
            EntityManager.Instance.Initialise();
            EntityCommandManager.Instance.Initialise();
            EntityCacheManager.Instance.Initialise();
            FactionManager.Instance.Initialise();
            GlobalMovementManager.Instance.Initialise();

            GlobalCinematicManager.Instance.Initialise();
            ChatFormatManager.Instance.Initialise();
            GlobalChatManager.Instance.Initialise(); // must be initialised before guilds
            GlobalAchievementManager.Instance.Initialise(); // must be initialised before guilds
            GlobalGuildManager.Instance.Initialise(); // must be initialised before residences
            CharacterManager.Instance.Initialise(); // must be initialised before residences
            GlobalResidenceManager.Instance.Initialise();
            GlobalGuildManager.Instance.ValidateCommunityResidences();
            
            DamageCalculator.Instance.Initialise();

            AssetManager.Instance.Initialise();
            ItemManager.Instance.Initialise();
            PrerequisiteManager.Instance.Initialise();
            GlobalSpellManager.Instance.Initialise();
            GlobalQuestManager.Instance.Initialise();

            GlobalStorefrontManager.Instance.Initialise();
            ServerManager.Instance.Initialise(RealmContext.Instance.RealmId);

            TextFilterManager.Instance.Initialise();

            CustomisationManager.Instance.Initialise();

            ShutdownManager.Instance.Initialise(WorldServer.Shutdown);
            LoginQueueManager.Instance.Initialise(CharacterHandler.SendCharacterListPackets);

            // initialise world after all assets have loaded but before any network or command handlers might be invoked
            worldManager.Initialise(lastTick =>
            {
                // NetworkManager must be first and MapManager must come before everything else
                NetworkManager<WorldSession>.Instance.Update(lastTick);
                MapManager.Instance.Update(lastTick);

                BuybackManager.Instance.Update(lastTick);
                GlobalQuestManager.Instance.Update(lastTick);
                GlobalGuildManager.Instance.Update(lastTick);
                GlobalResidenceManager.Instance.Update(lastTick); // must be after guild update
                GlobalChatManager.Instance.Update(lastTick);
                LoginQueueManager.Instance.Update(lastTick);
                ShutdownManager.Instance.Update(lastTick);

                scriptManager.Update(lastTick);

                // process commands after everything else in the tick has processed
                CommandManager.Instance.Update(lastTick);
            });

            // initialise network and command managers last to make sure the rest of the server is ready for invoked handlers
            MessageManager.Instance.Initialise();
            NetworkManager<WorldSession>.Instance.Initialise(SharedConfiguration.Instance.Get<NetworkConfig>());

            CommandManager.Instance.Initialise();

            log.LogInformation("Started!");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stop <see cref="WorldServer"/> and any related resources.
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            log.LogInformation("Stopping...");

            // stop network manager listening for incoming connections
            // it is still possible for incoming packets to be parsed though won't be handled once the world thread is stopped
            NetworkManager<WorldSession>.Instance.Shutdown();

            // stop command manager listening for commands
            CommandManager.Instance.Shutdown();

            // stop server manager pinging other servers
            ServerManager.Instance.Shutdown();

            // stop world manager processing the world thread
            // at this point no incoming packets will be handled
            worldManager.Shutdown();

            // save residences, guilds and players to the database
            GlobalResidenceManager.Instance.Shutdown();
            GlobalGuildManager.Instance.Shutdown();

            foreach (WorldSession worldSession in NetworkManager<WorldSession>.Instance)
                worldSession.Player?.SaveDirect();

            log.LogInformation("Stopped!");
            return Task.CompletedTask;
        }
    }
}
