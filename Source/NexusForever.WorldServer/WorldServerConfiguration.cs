using System.Collections.Generic;
using NexusForever.Shared.Configuration;

namespace NexusForever.WorldServer
{
    public class WorldServerConfiguration
    {
        public struct MapConfig
        {
            public string MapPath { get; set; }
            public List<ushort> PrecacheBaseMaps { get; set; }
            public List<ushort> PrecacheMapSpawns { get; set; }
            public uint? GridActionThreshold { get; set; }
            public double? GridUnloadTimer { get; set; }
        }

        public NetworkConfig Network { get; set; }
        public DatabaseConfig Database { get; set; }
        public MapConfig Map { get; set; }
        public bool UseCache { get; set; } = false;
        public ushort RealmId { get; set; }
        public uint LengthOfInGameDay { get; set; }
    }
}
