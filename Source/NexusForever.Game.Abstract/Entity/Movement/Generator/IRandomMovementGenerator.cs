using System.Numerics;
using NexusForever.Game.Abstract.Map;

namespace NexusForever.Game.Abstract.Entity.Movement.Generator
{
    public interface IRandomMovementGenerator : IMovementGenerator
    {
        Vector3 Begin { get; set; }
        Vector3 Leash { get; set; }
        IBaseMap Map { get; set; }
        float Range { get; set; }
    }
}