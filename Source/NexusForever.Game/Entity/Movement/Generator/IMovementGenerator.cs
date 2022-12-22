using System.Numerics;

namespace NexusForever.Game.Entity.Movement.Generator
{
    public interface IMovementGenerator
    {
        List<Vector3> CalculatePath();
    }
}
