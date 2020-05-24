using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using NexusForever.Database;
using NexusForever.Database.World.Model;
using NexusForever.Shared;
using NexusForever.Shared.Database;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.ServerConfig.Static;
using NLog;

namespace NexusForever.WorldServer.Game.ServerConfig
{
    class ServerConfigManager : Singleton<ServerConfigManager>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private ServerConfig config;
        public void Initialise()
        {
            log.Info("Initialising Serer Configuration...");
            config = new ServerConfig(DatabaseManager.Instance.WorldDatabase.GetActiveServerConfig());
        }

        public CharacterCreationLocation GetCharacterCreationLocationByCreationEntry(CharacterCreationEntry creationEntry)
        {
            return config.CharacterCreationLocations.FirstOrDefault(s =>
                s.CharacterCreation == (CharacterCreation)creationEntry.CharacterCreationStartEnum &&
                s.Race == (Race)creationEntry.RaceId &&
                s.Faction == (Faction)creationEntry.FactionId);
        }
    }
}
