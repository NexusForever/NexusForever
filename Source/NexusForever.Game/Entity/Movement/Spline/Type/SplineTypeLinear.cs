using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Spline;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Type;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Type
{
    public class SplineTypeLinear : ISplineType
    {
        public SplineType Type => SplineType.Linear;

        /// <summary>
        /// Total length of the spline.
        /// </summary>
        public float Length { get; private set; }

        /// <summary>
        /// Total length of delays in the spline.
        /// </summary>
        public float DelayLength { get; private set; }

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
        private void CalculateLengths(List<ISplinePoint> points)
        {
            for (int i = 1; i < points.Count - 1; i++)
                DelayLength += points[i].TemplatePoint.Delay ?? 0f;

            for (int i = 1; i < points.Count - 2; i++)
            {
                points[i].Lengths.Add(new SplinePointLength
                {
                    Distance = Length,
                    Delay    = points[i].TemplatePoint.Delay ?? 0f,
                });

                Length += Vector3.Distance(points[i + 1].TemplatePoint.Position, points[i].TemplatePoint.Position);
            }

            points[^2].Lengths.Add(new SplinePointLength
            {
                Delay    = points[^2].TemplatePoint.Delay ?? 0f,
                Distance = Length,
            });
        }

        /// <summary>
        /// Get the interpolated position between two points.
        /// </summary>
        public Vector3 GetInterpolatedPosition(ISplinePoint point, ISplinePoint point2, float t)
        {
            return Vector3.Lerp(point.TemplatePoint.Position, point2.TemplatePoint.Position, t);
        }

        /// <summary>
        /// Get the interpolated rotation between two points.
        /// </summary>
        public Vector3 GetInterpolatedRotation(ISplinePoint point, ISplinePoint point2, float t)
        {
            return point2.TemplatePoint.Position - point.TemplatePoint.Position;
        }
    }
}
