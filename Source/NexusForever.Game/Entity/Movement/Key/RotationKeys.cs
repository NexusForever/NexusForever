using System.Numerics;

namespace NexusForever.Game.Entity.Movement.Key
{
    public class RotationKeys : Keys<Vector3>
    {
        /// <summary>
        /// Calculate the interpolcated <see cref="Vector3"/> rotation value at the current time.
        /// </summary>
        public override Vector3 CalculateInterpolated(Vector3 v1, Vector3 v2, float t)
        {
            Quaternion q1 = Quaternion.CreateFromYawPitchRoll(v1.X, v1.Y, v1.Z);
            Quaternion q2 = Quaternion.CreateFromYawPitchRoll(v2.X, v2.Y, v2.Z);
            Quaternion q3 = Quaternion.Lerp(q1, q2, t);
            return q3.ToEuler();
        }
    }
}
