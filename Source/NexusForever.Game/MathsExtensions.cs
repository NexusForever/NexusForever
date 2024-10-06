using System.Numerics;

namespace NexusForever.Game
{
    public static class MathsExtensions
    {
        public static float PI2 = MathF.PI * 2;

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
        public static float NormaliseRotationRadians(this float radians)
        {
            // normalize between 0 and 2*PI
            float r = radians % (2 * MathF.PI); 

            // normalize between PI and -PI
            if (r > MathF.PI)
                r -= 2 * MathF.PI;
            else if (radians < -Math.PI)
                r += 2 * MathF.PI;

            return r;
        }

        /// <summary>
        /// Convert <see cref="Quaternion"/> to <see cref="Vector3"/> in degrees.
        /// </summary>
        public static Vector3 ToEulerDegrees(this Quaternion q)
        {
            Vector3 vector = ToEuler(q);
            vector.X = vector.X.ToDegrees();
            vector.Y = vector.Y.ToDegrees();
            vector.Z = vector.Z.ToDegrees();
            return vector;
        }

        /// <summary>
        /// Convert <see cref="Quaternion"/> to <see cref="Vector3"/> in degrees.
        /// </summary>
        public static Vector3 ToEuler(this Quaternion q)
        {
            float xx = q.X * q.X;
            float xy = q.X * q.Y;
            float xz = q.X * q.Z;
            float xw = q.X * q.W;
            float yy = q.Y * q.Y;
            float yz = q.Y * q.Z;
            float yw = q.Y * q.W;
            float zz = q.Z * q.Z;
            float zw = q.Z * q.W;

            float p = MathF.Asin(-2f * (yz - xw));
            float y = MathF.Atan2(2f * (xz + yw), 1f - 2f * (xx + yy));
            float r = MathF.Atan2(2f * (xy + zw), 1f - 2f * (xx + zz));
            return new Vector3(y, p, r);
        }

        /// <summary>
        /// Returns a <see cref="Vector3"/> which has been offset by a random angle and distance between 0 and supplied max range.
        /// </summary>
        public static Vector3 GetRandomPoint2D(this Vector3 v, float maxRange)
        {
            var random = new Random();
            return GetPoint2D(v, (float)random.NextDouble() * PI2, (float)random.NextDouble() * maxRange);
        }

        /// <summary>
        /// Returns a <see cref="Vector3"/> which has been offset by supplied angle and distance. Only for use in Telegraphs.
        /// </summary>
        public static Vector3 GetPointForTelegraph(this Vector3 v, float angle, float distance)
        {
            float x = v.X + MathF.Cos(angle) * distance;
            float z = v.Z - MathF.Sin(angle) * distance;
            return new Vector3(x, v.Y, z);
        }

        /// <summary>
        /// Returns a <see cref="Vector3"/> which has been offset by supplied angle and distance.
        /// </summary>
        public static Vector3 GetPoint2D(this Vector3 v, float angle, float distance)
        {
            float x = v.X + MathF.Cos(angle) * distance;
            float z = v.Z + MathF.Sin(angle) * distance;
            return new Vector3(x, v.Y, z);
        }

        /// <summary>
        /// Returns the distance between this <see cref="Vector3"/> and a target <see cref="Vector3"/>.
        /// </summary>
        public static float GetDistance(this Vector3 v1, Vector3 v2)
        {
            return MathF.Sqrt(MathF.Pow(v1.X - v2.X, 2) + MathF.Pow(v1.Z - v2.Z, 2));
        }

        /// <summary>
        /// Linearly interpolate between two values.
        /// </summary>
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }
}
