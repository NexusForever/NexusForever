using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public interface IInstancedMap : IMap
    {
        IMap CreateInstance(MapInfo info, Player player);
    }
}
