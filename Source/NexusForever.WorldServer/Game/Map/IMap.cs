using System.Numerics;
using NexusForever.Shared;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public interface IMap : IUpdate
    {
        void Initialise(MapInfo info, Player player);
        void EnqueueAdd(GridEntity entity, Vector3 position);
    }
}
