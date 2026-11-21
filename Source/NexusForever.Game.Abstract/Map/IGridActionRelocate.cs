using System.Numerics;

namespace NexusForever.Game.Abstract.Map
{
    public interface IGridActionRelocate : IGridAction
    {
        Vector3 Vector { get; init; }
        OnRelocateDelegate Callback { get; init; }

        /// <summary>
        /// Movement relocation: only one is queued per entity, it moves to the latest position.
        /// </summary>
        bool Coalesce { get; init; }
    }
}