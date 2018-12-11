using NexusForever.Shared.GameTable.Model;

namespace NexusForever.WorldServer.Game.Map
{
    public class MapInfo
    {
        public WorldEntry Entry { get; }
        public uint InstanceId { get; }
        public ulong ResidenceId { get; }

        public MapInfo(WorldEntry entry, uint instanceId = 0u, ulong residenceId = 0ul)
        {
            Entry       = entry;
            InstanceId  = instanceId;
            ResidenceId = residenceId;
        }
    }
}
