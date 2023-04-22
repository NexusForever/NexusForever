using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Spline;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Type;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Type
{
    [SplineType(SplineType.Linear)]
    public class SplineTypeLinear : ISplineType
    {
        public uint BottomIndex { get; private set; }
        public uint TopIndex { get; private set; }
        public uint BottomReverseIndex { get; private set; }
        public uint TopReverseIndex { get; private set; }

        public void Initialise(uint points)
        {
            BottomIndex        = 0u;
            TopIndex           = points - 2u;
            BottomReverseIndex = points - 1u;
            TopReverseIndex    = 1u;
        }

        /// <summary>
        /// Get the interpolated <see cref="Vector3"/> between supplied points in <see cref="SplineDirection"/> at normalised (0 - totalLength) position p.
        /// </summary>
        public Vector3 GetInterpolatedPoint(SplineDirection direction, float p, List<ISplinePoint> points)
        {
            if (direction == SplineDirection.Forward)
            {
                for (int i = (int)BottomIndex; i <= TopIndex; i++)
                {
                    if (p <= points[i].Length)
                    {
                        float t = p / points[i].Length;
                        return GetInterpolatedPoint(t, points[i], points[i + 1]);
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
                        return GetInterpolatedPoint(1f - t, points[i - 1], points[i]);
                    }

                    p -= points[i - 1].Length;
                }
            }

            return Vector3.Zero;
        }

        /// <summary>
        /// Get the interpolated <see cref="Vector3"/> between supplied points at normalised (0-1) distance t.
        /// </summary>
        public Vector3 GetInterpolatedPoint(float t, params ISplinePoint[] points)
        {
            return Vector3.Lerp(points[0].Position, points[1].Position, t);
        }

        /// <summary>
        /// Calculate the length of each segment and returns the total length.
        /// </summary>
        public float CalculateLengths(List<ISplinePoint> points)
        {
            float totalLength = 0f;
            for (int i = (int)BottomIndex; i <= TopIndex; i++)
            {
                ISplinePoint p0 = points[i];
                ISplinePoint p1 = points[i + 1];

                p0.Length = Vector3.Distance(p0.Position, p1.Position);
                totalLength += p0.Length;
            }

            return totalLength;
        }
    }
}
