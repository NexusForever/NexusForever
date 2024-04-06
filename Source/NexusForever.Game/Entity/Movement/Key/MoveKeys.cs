using System.Numerics;

namespace NexusForever.Game.Entity.Movement.Key
{
    public class MoveKeys : Keys<Vector3>
    {
        /// <summary>
        /// Calculate the interpolcated <see cref="Vector3"/> move value at the current time.
        /// </summary>
        public override Vector3 CalculateInterpolated(Vector3 v1, Vector3 v2, float t)
        {
            return Vector3.Lerp(v1, v2, t);
        }
    }
}
