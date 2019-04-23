using System;
using System.Collections.Generic;
using System.Numerics;
using NexusForever.WorldServer.Game.Entity.Movement.Spline.Static;

namespace NexusForever.WorldServer.Game.Entity.Movement.Spline.Implementation
{
    [SplineType(SplineType.Linear)]
    public class SplineTypeLinear : SplineTypeBase
    {
        public override void Initialise(uint points)
        {
            BottomIndex        = 0u;
            TopIndex           = points - 2u;
            BottomReverseIndex = points - 1u;
            TopReverseIndex    = 1u;
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

        public override Vector3 GetInterpolatedPoint(float t, params SplinePoint[] points)
        {
            return Vector3.Lerp(points[0].Position, points[1].Position, t);
        }

        public override float CalculateLengths(List<SplinePoint> points)
        {
            float totalLength = 0f;
            for (int i = (int)BottomIndex; i <= TopIndex; i++)
            {
                SplinePoint p0 = points[i];
                SplinePoint p1 = points[i + 1];

                p0.Length = Vector3.Distance(p0.Position, p1.Position);
                totalLength += p0.Length;
            }

            return totalLength;
        }
    }
}
