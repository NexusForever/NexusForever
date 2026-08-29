using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NexusForever.Database;
using NexusForever.Database.Configuration.Model;
using NexusForever.Game;
using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Abstract.PublicEvent;
using NexusForever.Game.Achievement;
using NexusForever.Game.Character;
using NexusForever.Game.Cinematic;
using NexusForever.Game.Customisation;
using NexusForever.Game.Entity;
using NexusForever.Game.Guild;
using NexusForever.Game.Housing;
using NexusForever.Game.Map;
using NexusForever.Game.Quest;
using NexusForever.Game.RBAC;
using NexusForever.Game.Reputation;
using NexusForever.Game.Server;
using NexusForever.Game.Spell;
using NexusForever.Game.Storefront;
using NexusForever.GameTable;
using NexusForever.GameTable.Text.Filter;
using NexusForever.GameTable.Text.Search;
using NexusForever.Network.Message;
using NexusForever.Network.Session;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message;
using NexusForever.Script;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Service
{
    public class HostedService : IHostedService
    {
        #region Dependency Injection

        private readonly ILogger log;

        private readonly IScriptManager scriptManager;
        private readonly ILoginQueueManager loginQueueManager;
        private readonly INetworkManager<IWorldSession> networkManager;
        private readonly IMessageManager messageManager;
        private readonly IMatchingManager matchingManager;
        private readonly IMatchManager matchManager;
        private readonly IPublicEventTemplateManager publicEventManager;
        private readonly IChatFormatManager chatFormatManager;
        private readonly IWorldManager worldManager;

        public HostedService(
            ILogger<IHostedService> log,
            IServiceProvider serviceProvider,
            IScriptManager scriptManager,
            ILoginQueueManager loginQueueManager,
            INetworkManager<IWorldSession> networkManager,
            IMessageManager messageManager,
            IMatchingManager matchingManager,
            IMatchManager matchManager,
            IPublicEventTemplateManager publicEventManager,
            IChatFormatManager chatFormatManager,
            IWorldManager worldManager)
        {
            this.log               = log;

            LegacyServiceProvider.Provider = serviceProvider;

            this.scriptManager      = scriptManager;
            this.loginQueueManager  = loginQueueManager;
            this.networkManager     = networkManager;
            this.messageManager     = messageManager;
            this.matchingManager    = matchingManager;
            this.matchManager       = matchManager;
            this.publicEventManager = publicEventManager;
            this.chatFormatManager  = chatFormatManager;
            this.worldManager       = worldManager;
        }

        #endregion

        /// <summary>
        /// Start <see cref="WorldServer"/> and any related resources.
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            log.LogInformation("Starting...");

            SharedConfiguration.Instance.Initialise<WorldServerConfiguration>();

            DatabaseManager.Instance.Initialise(SharedConfiguration.Instance.Get<DatabaseConfig>());
            DatabaseManager.Instance.Migrate();

            RealmContext.Instance.Initialise();

            // RBACManager must be initialised before CommandManager
            RBACManager.Instance.Initialise();

            DisableManager.Instance.Initialise();

            scriptManager.Initialise();

            await GameTableManager.Instance.Initialise();
            publicEventManager.Initialise();
            MapIOManager.Instance.Initialise();
            SearchManager.Instance.Initialise();
            EntityManager.Instance.Initialise();
            EntityCommandManager.Instance.Initialise();
            EntityCacheManager.Instance.Initialise();
            FactionManager.Instance.Initialise();

            GlobalCinematicManager.Instance.Initialise();
            chatFormatManager.Initialise();
            GlobalAchievementManager.Instance.Initialise(); // must be initialised before guilds
            GlobalGuildManager.Instance.Initialise(); // must be initialised before residences
            CharacterManager.Instance.Initialise(); // must be initialised before residences
            GlobalResidenceManager.Instance.Initialise();

            AssetManager.Instance.Initialise();
            ItemManager.Instance.Initialise();
            GlobalSpellManager.Instance.Initialise();
            GlobalQuestManager.Instance.Initialise();

            GlobalStorefrontManager.Instance.Initialise();
            ServerManager.Instance.Initialise(RealmContext.Instance.RealmId);

            TextFilterManager.Instance.Initialise();

            CustomisationManager.Instance.Initialise();

            ShutdownManager.Instance.Initialise(WorldServer.Shutdown);

            matchingManager.Initialise();

            messageManager.RegisterNetworkManagerMessagesAndHandlers();
            messageManager.RegisterNetworkManagerWorldMessages();
            messageManager.RegisterNetworkManagerWorldHandlers();

            // initialise world after all assets have loaded but before any network or command handlers might be invoked
            worldManager.Initialise(lastTick =>
            {
                // NetworkManager must be first and MapManager must come before everything else
                networkManager.Update(lastTick);
                MapManager.Instance.Update(lastTick);

                BuybackManager.Instance.Update(lastTick);
                GlobalQuestManager.Instance.Update(lastTick);
                GlobalGuildManager.Instance.Update(lastTick);
                GlobalResidenceManager.Instance.Update(lastTick); // must be after guild update

                loginQueueManager.Update(lastTick);
                matchingManager.Update(lastTick);
                matchManager.Update(lastTick);

                scriptManager.Update(lastTick);

                ShutdownManager.Instance.Update(lastTick);

                // process commands after everything else in the tick has processed
                CommandManager.Instance.Update(lastTick);
            });

            // initialise network and command managers last to make sure the rest of the server is ready for invoked handlers
            networkManager.Initialise();
            networkManager.Start();

            CommandManager.Instance.Initialise();

            log.LogInformation("Started!");
        }

        /// <summary>
        /// Stop <see cref="WorldServer"/> and any related resources.
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            log.LogInformation("Stopping...");

            // stop network manager listening for incoming connections
            // it is still possible for incoming packets to be parsed though won't be handled once the world thread is stopped
            networkManager.Shutdown();

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

            foreach (IWorldSession worldSession in networkManager)
                worldSession.Player?.SaveDirect();

            log.LogInformation("Stopped!");
            return Task.CompletedTask;
        }
    }
}
