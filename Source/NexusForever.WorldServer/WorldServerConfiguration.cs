using NexusForever.Shared.Configuration;
using System.Collections.Generic;

namespace NexusForever.WorldServer
{
    public class WorldServerConfiguration
    {
        public struct MapConfig
        {
            public string MapPath { get; set; }
        }

        public struct Command
        {
            public string Name { get; set; }
            public CommandData Data { get; set; }
        }

        public struct CommandData
        {
            public int MinimumStatus;
            public IEnumerable<Command> SubCommands { get; set; }
        }

        public NetworkConfig Network { get; set; }
        public DatabaseConfig Database { get; set; }
        public MapConfig Map { get; set; }
        public IEnumerable<Command> Commands { get; set; }
        public bool UseCache { get; set; } = false;
        public ushort RealmId { get; set; }
    }
}
