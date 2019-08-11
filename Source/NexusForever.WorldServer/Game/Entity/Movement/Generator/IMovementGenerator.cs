using System.Collections.Generic;
using System.Numerics;

namespace NexusForever.WorldServer.Game.Entity.Movement.Generator
{
    public interface IMovementGenerator
    {
        List<Vector3> CalculatePath();
    }
}
