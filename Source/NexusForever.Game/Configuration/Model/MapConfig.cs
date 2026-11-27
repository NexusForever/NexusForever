using NexusForever.Shared.Configuration;

namespace NexusForever.Game.Configuration.Model
{
    [ConfigurationBind]
    public class MapConfig
    {
        public string MapPath { get; set; } = "map";
        public List<ushort> PrecacheBaseMaps { get; set; }
        public List<ushort> PrecacheMapSpawns { get; set; }
        public bool SynchronousUpdate { get; set; } = false;
        public uint? GridActionThreshold { get; set; } = 100u;
        public uint? GridActionMaxRetry { get; set; } = 5u;
        public double? GridUnloadTimer { get; set; } = 600u;
        public uint? MaxInstances { get; set; } = 10u;

        /// <summary>
        /// Minimum delay before a persistent open-world creature respawns. Zero disables respawning.
        /// </summary>
        public double StaticCreatureRespawnSeconds { get; set; } = 30d;
    }
}