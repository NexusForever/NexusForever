using System.Numerics;
using NexusForever.Shared;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public interface IMapInstance : IMap
    {
        void CreateInstance(MapInfo info, Player player);
    }
}
