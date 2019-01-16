using System;
using System.Numerics;

namespace NexusForever.Shared
{
    public static class MathsExtensions
    {
        /// <summary>
        /// Convert degrees to radians.
        /// </summary>
        public static float ToRadians(this float degrees)
        {
            return degrees * MathF.PI / 180f;
        }

        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        public static float ToDegrees(this float radians)
        {
            return radians * 180f / MathF.PI;
        }

        /// <summary>
        /// Get angle in radians between <see cref="Vector3"/> a and b.
        /// </summary>
        public static float GetAngle(this Vector3 a, Vector3 b)
        {
            float angle = MathF.Atan2(a.Z - b.Z, b.X - a.X);

            // normalise so that left is positive up is neutral and right negative
            angle -= MathF.PI / 2;
            if (angle < -MathF.PI)
                angle = MathF.PI * 2 + angle;

            return angle;
        }

        /// <summary>
        /// Normalise radians between PI and -PI.
        /// </summary>
        public static float NormaliseRadians(this float radians)
        {
            return radians % MathF.PI;
        }

        /// <summary>
        /// Normalise radians between 180 and -180.
        /// </summary>
        public static float NormaliseDegrees(this float degrees)
        {
            return degrees % 180f;
        }
    }
}
