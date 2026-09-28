using System.Numerics;
using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Script.Instance.Adventure.TheHycrestInsurrection
{
    /// <summary>
    /// Implemented by the intro event script: where arriving players enter the map.
    /// </summary>
    public interface IHycrestArrivalScript
    {
        /// <summary>
        /// Return the position on the drop ship's deck <paramref name="player"/> enters the map at, null for the default entrance.
        /// </summary>
        Vector3? GetEntryPosition(IPlayer player);
    }
}
