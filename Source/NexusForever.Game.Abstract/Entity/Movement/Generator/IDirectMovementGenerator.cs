using System.Numerics;
using NexusForever.Game.Abstract.Map;

namespace NexusForever.Game.Abstract.Entity.Movement.Generator
{
    public interface IDirectMovementGenerator : IMovementGenerator
    {
        Vector3 Begin { get; set; }
        Vector3 Final { get; set; }
        IBaseMap Map { get; set; }
    }
}