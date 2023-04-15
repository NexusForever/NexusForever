using System.Numerics;

namespace NexusForever.Game.Abstract.Entity.Movement.Generator
{
    public interface IMovementGenerator
    {
        List<Vector3> CalculatePath();
    }
}
