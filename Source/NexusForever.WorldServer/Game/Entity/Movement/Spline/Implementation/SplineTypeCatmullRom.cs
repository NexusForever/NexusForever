using System.Collections.Generic;
using System.Numerics;
using NexusForever.WorldServer.Game.Entity.Movement.Spline.Static;

namespace NexusForever.WorldServer.Game.Entity.Movement.Spline.Implementation
{
    [SplineType(SplineType.CatmullRom)]
    public class SplineTypeCatmullRom : SplineTypeBase
    {
        /// <summary>
        /// Precision step size between two interpolation points on the spline.
        /// </summary>
        private const float Precision = 0.005f;

        public override void Initialise(uint points)
        {
            BottomIndex        = 1u;
            TopIndex           = points - 3u;
            BottomReverseIndex = points - 2u;
            TopReverseIndex    = 2u;
        }

        public override Vector3 GetInterpolatedPoint(SplineDirection direction, float p, List<SplinePoint> points)
        {
            if (direction == SplineDirection.Forward)
            {
                for (int i = (int)BottomIndex; i <= TopIndex; i++)
                {
                    if (p <= points[i].Length)
                    {
                        float t = p / points[i].Length;
                        return GetInterpolatedPoint(t, points[i - 1], points[i], points[i + 1], points[i + 2]);
                    }

                    p -= points[i].Length;
                }
            }
            else
            {
                for (int i = (int)BottomReverseIndex; i >= TopReverseIndex; i--)
                {
                    if (p <= points[i - 1].Length)
                    {
                        float t = p / points[i - 1].Length;
                        return GetInterpolatedPoint(1f - t, points[i - 2], points[i - 1], points[i], points[i + 1]);
                    }

                    p -= points[i - 1].Length;
                }
            }

            return Vector3.Zero;
        }

        public override Vector3 GetInterpolatedPoint(float t, params SplinePoint[] points)
        {
            Vector3 a = 2f * points[1].Position;
            Vector3 b = points[2].Position - points[0].Position;
            Vector3 c = 2f * points[0].Position - 5f * points[1].Position + 4f * points[2].Position - points[3].Position;
            Vector3 d = -points[0].Position + 3f * points[1].Position - 3f * points[2].Position + points[3].Position;
            return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
        }

        public override float CalculateLengths(List<SplinePoint> points)
        {
            float totalLength = 0f;
            for (int i = (int)BottomIndex; i <= TopIndex; i++)
            {
                SplinePoint p0 = points[i - 1];
                SplinePoint p1 = points[i];
                SplinePoint p2 = points[i + 1];
                SplinePoint p3 = points[i + 2];

                float length = 0f;
                Vector3 currentPoint = GetInterpolatedPoint(0f, p0, p1, p2, p3);
                for (float t = Precision; t < 1.0f; t += Precision)
                {
                    Vector3 nextPoint = GetInterpolatedPoint(t, p0, p1, p2, p3);
                    length += Vector3.Distance(currentPoint, nextPoint);
                    currentPoint = nextPoint;
                }

                p1.Length = length;
            }

            return totalLength;
        }
    }
}
