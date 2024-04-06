using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Spline;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Type;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Type
{
    public class SplineTypeCatmullRom : ISplineType
    {
        public SplineType Type => SplineType.CatmullRom;

        /// <summary>
        /// Total length of the spline.
        /// </summary>
        public float Length { get; protected set; }

        /// <summary>
        /// Total length of delays in the spline.
        /// </summary>
        public float DelayLength { get; private set; }

        private Vector3[] distanceToLeft;
        private Vector3[] distanceToRight;

        /// <summary>
        /// Initialise the spline with a collection of <see cref="ISplinePoint"/>.
        /// </summary>
        public void Initialise(List<ISplinePoint> points)
        {
            if (points.Count < 4)
                throw new ArgumentOutOfRangeException();

            CalculateLengths(points);
        }

        /// <summary>
        /// Calculate the length of each segment and total spline length.
        /// </summary>
        /// <remarks>
        /// This is based on client code, blame Rawaho if these calculations are wrong :)
        /// </remarks>
        private void CalculateLengths(List<ISplinePoint> points)
        {
            CalculateLeftRight(points);

            for (int i = 1; i < points.Count - 1; i++)
                DelayLength += points[i].TemplatePoint.Delay ?? 0f;

            for (int i = 1; i < points.Count - 2; i++)
            {
                points[i].Lengths.Add(new SplinePointLength
                {
                    Distance = Length,
                    Delay    = points[i].TemplatePoint.Delay ?? 0f,
                });

                float segmentLength = 0f;
                var v50 = Vector3.Distance(points[i + 1].TemplatePoint.Position, points[i].TemplatePoint.Position);
                CalculateCatmullRomLength(ref segmentLength, points[i], points[i + 1], points[i].TemplatePoint.Position, points[i + 1].TemplatePoint.Position, 0f, 1.0f, v50);

                Length += segmentLength;
            }

            points[^2].Lengths.Add(new SplinePointLength
            {
                Delay    = points[^2].TemplatePoint.Delay ?? 0f,
                Distance = Length,
            });
        }

        private void CalculateCatmullRomLength(ref float segmentLength, ISplinePoint point, ISplinePoint point2, Vector3 left, Vector3 right, float tLeft, float tRight, float distance)
        {
            if (distance < 0.000099999997f)
            {
                segmentLength += distance;
                return;
            }

            float t = (tLeft + tRight) * 0.5f;
            Vector3 position = GetInterpolatedPosition(point, point2, t);

            float leftDistance  = Vector3.Distance(position, left);
            float rightDistance = Vector3.Distance(right, position);
            if (leftDistance + rightDistance <= distance + 0.1f)
                segmentLength += distance;
            else
            {
                CalculateCatmullRomLength(ref segmentLength, point, point2, left, position, tLeft, t, leftDistance);

                point.Lengths.Add(new SplinePointLength
                {
                    Distance = Length + segmentLength,
                    T        = t
                });

                CalculateCatmullRomLength(ref segmentLength, point, point2, position, right, t, tRight, rightDistance);
            }
        }

        private void CalculateLeftRight(List<ISplinePoint> points)
        {
            distanceToLeft  = new Vector3[points.Count];
            distanceToRight = new Vector3[points.Count];

            for (int i = 1; i < points.Count - 1; i++)
            {
                var distanceLeft  = Vector3.Subtract(points[i].TemplatePoint.Position, points[i - 1].TemplatePoint.Position);
                var distanceRight = Vector3.Subtract(points[i + 1].TemplatePoint.Position, points[i].TemplatePoint.Position);
                var dotLeft       = Vector3.Dot(distanceLeft, distanceLeft);
                var dotRight      = Vector3.Dot(distanceRight, distanceRight);

                if (dotLeft > 0.000099999997f && dotRight > 0.000099999997f)
                {
                    Vector3 v6 = dotRight * distanceLeft + dotLeft * distanceRight;
                    Vector3 v4 = v6 * v6;

                    float vectorSum = v4.X + v4.Y + v4.Z;
                    float v5 = MathF.ReciprocalSqrtEstimate(vectorSum);
                    float v2 = MathF.Max((3f - vectorSum * v5 * v5) * 0.5f * v5, 0f);

                    distanceLeft  = v2 * v6 * MathF.Sqrt(dotLeft);
                    distanceRight = v2 * v6 * MathF.Sqrt(dotRight);
                }

                distanceToLeft[i]  = distanceLeft;
                distanceToRight[i] = distanceRight;
            }
        }

        /// <summary>
        /// Get the interpolated position between two points.
        /// </summary>
        public Vector3 GetInterpolatedPosition(ISplinePoint point, ISplinePoint point2, float t)
        {
            float a = t * t * t * 2.0f - t * t * 3.0f + 1.0f;
            float b = t * t * t - t * t;
            float c = t * t * t - t * t * 2f + t;
            float d = t * t * 3f - t * t * t * 2.0f;

            return point.TemplatePoint.Position * a + point2.TemplatePoint.Position * d
                + distanceToRight[point.Index] * c + distanceToLeft[point2.Index] * b;
        }

        /// <summary>
        /// Get the interpolated rotation between two points.
        /// </summary>
        public Vector3 GetInterpolatedRotation(ISplinePoint point, ISplinePoint point2, float t)
        {
            float a = t * t * 6.0f - t * 6.0f;
            float b = t * t * 3.0f - t * 4.0f + 1.0f;
            float c = t * t * 3.0f - t * 2.0f;
            float d = t * 6.0f - t * t * 6.0f;

            return (point.TemplatePoint.Position * a) + (point2.TemplatePoint.Position * d)
                + (distanceToRight[point.Index] * b) + (distanceToLeft[point2.Index] * c);
        }
    }
}
