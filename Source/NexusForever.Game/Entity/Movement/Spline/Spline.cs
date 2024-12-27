using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Spline;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Mode;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Template;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Type;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline
{
    public class Spline : ISpline
    {
        /// <summary>
        /// Returns if the spline has been finalised.
        /// </summary>
        public bool IsFinialised { get; private set; }

        public List<ISplinePoint> Points { get; } = new();
        public ISplineType Type { get; private set; }
        public ISplineMode Mode { get; private set; }
        public float Speed { get; private set; }
        public SplineDirection Direction { get; private set; }

        /// <summary>
        /// Position on the spline.
        /// </summary>
        /// <remarks>
        /// Value will be between 0 and the total length of the spline.
        /// </remarks>
        public float Position { get; private set; }

        /// <summary>
        /// Offset on the spline.
        /// </summary>
        /// <remarks>
        /// Value will be between 0 and t max (usually 1).
        /// </remarks>
        public float Offset => Position / (Type.Length + (Type.DelayLength * Speed));

        public float Unknown30 { get; private set; }

        private Vector3? formation;

        #region Dependency Injection

        private readonly ISplineTypeFactory splineTypeFactory;
        private readonly ISplineModeFactory splineModeFactory;

        public Spline(
            ISplineTypeFactory splineTypeFactory,
            ISplineModeFactory splineModeFactory)
        {
            this.splineTypeFactory = splineTypeFactory;
            this.splineModeFactory = splineModeFactory;
        }

        #endregion

        /// <summary>
        /// Initialise the spline with the supplied <see cref="ISplineTemplate"/>, <see cref="SplineMode"/> and speed.
        /// </summary>
        public void Initialise(ISplineTemplate template, SplineMode mode, float speed)
        {
            if (speed < float.Epsilon)
                throw new ArgumentOutOfRangeException();

            Speed = speed;

            for (int i = 0; i < template.Points.Count; i++)
            {
                Points.Add(new SplinePoint
                {
                    Index         = i,
                    TemplatePoint = template.Points[i]
                });
            }

            Type = splineTypeFactory.Create(template.Type);
            Type.Initialise(Points);

            Mode = splineModeFactory.Create(mode);

            CalculateFrameTimes();
            CalculateOffsets();
        }

        /// <summary>
        /// Calculate frame times for each point without delay.
        /// </summary>
        private void CalculateFrameTimes()
        {
            float totalDelay = 0f;
            for (int i = 1; i < Points.Count - 1; i++)
            {
                ISplinePoint point = Points[i];
                point.FrameTime = (point.TemplatePoint.FrameTime ?? point.Lengths[0].Distance) - totalDelay;
                totalDelay += point.TemplatePoint.Delay ?? 0f;
            }
        }

        /// <summary>
        /// Calculate the offsets for each point based on speed.
        /// </summary>
        /// <remarks>
        /// This is based on client code, blame Rawaho if these calculations are wrong :)
        /// </remarks>
        private void CalculateOffsets()
        {
            float v12 = Type.Length / Speed;
            float v12WithDelay = v12 + Type.DelayLength;
            float v14 = v12 / Points[^2].FrameTime;
            float v15 = 1f / v12WithDelay;
            float totalDelay = 0f;

            for (int i = 1; i < Points.Count - 2; i++)
            {
                ISplinePoint point = Points[i];

                point.Offsets.Add(Math.Clamp(((point.FrameTime * v14) + totalDelay) * v15, 0.0f, 1.0f));

                float delay = point.TemplatePoint.Delay ?? 0.0f;
                totalDelay += delay;

                // if this segment has more than one length, calculate the offsets for the rest of the lengths
                if (point.Lengths.Count > 1)
                {
                    ISplinePoint point2 = Points[i + 1];

                    float v23 = (delay * v15) + point.Offsets[0];
                    float v25 = ((point2.FrameTime * v14) + totalDelay) * v15;
                    float v28 = point2.Lengths[0].Distance - point.Lengths[0].Distance;
                    float v29 = v28 <= 0.0000099999997f ? 0.0f : (v25 - v23) / v28;

                    for (int j = 1; j < point.Lengths.Count; j++)
                    {
                        float v31 = point.Lengths[j].Distance;
                        point.Offsets.Add(Math.Clamp(((v31 - point.Lengths[0].Distance) * v29) + v23, 0.0f, 1.0f));
                    }
                }
            }

            Points[^2].Offsets.Add(1.0f);

            // not really sure what this is... but it's used in the calculation of t
            Unknown30 = v15;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            if (IsFinialised)
                return;

            float delta = (float)(lastTick * Speed);
            Position += delta;

            ISplineModeInterpolatedOffset result = Mode.GetInterpolatedOffset(Offset);
            IsFinialised = result.Finalised;
            Direction    = result.Direction;
        }

        /// <summary>
        /// Calculate the two points the current offset is between.
        /// </summary>
        private void CalculatePoints(float offset, out ISplinePoint point, out int index, out ISplinePoint point2, out int index2)
        {
            point  = Points[1];
            point2 = Points[^2];
            index  = index2 = 0;

            for (int i = 1; i < Points.Count - 2; i++)
            {
                ISplinePoint item = Points[i];
                for (int j = 0; j < item.Offsets.Count; j++)
                {
                    if (item.Offsets[j] <= offset)
                    {
                        point = item;
                        index = j;
                    }
                    else
                    {
                        point2 = item;
                        index2 = j;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Calculate the current t value between two points.
        /// </summary>
        /// <remarks>
        /// This is based on client code, blame Rawaho if these calculations are wrong :)
        /// </remarks>
        private float CalculateT(float offset, ISplinePoint point, int index, ISplinePoint point2, int index2)
        {
            float tLength = point.Index == point2.Index ? point2.Lengths[index2].T : 1.0f;
            tLength -= point.Lengths[index].T;

            float v25 = 0f;
            if (point2 == Points[^2] && index2 == point2.Offsets.Count - 1)
                v25 = point2.Lengths[index2].Delay * Unknown30;

            float v26 = (point.Lengths[index].Delay * Unknown30) + point.Offsets[index];
            float v54 = (offset - v26) * (1.0f / (point2.Offsets[index2] - v25 - v26));

            return (tLength * Math.Clamp(v54, 0.0f, 1.0f)) + point.Lengths[index].T;
        }

        /// <summary>
        /// Get current position on the spline.
        /// </summary>
        public Vector3 GetPosition()
        {
            ISplineModeInterpolatedOffset result = Mode.GetInterpolatedOffset(Offset);
            CalculatePoints(result.Offset, out ISplinePoint point, out int index, out ISplinePoint point2, out int index2);

            float t = CalculateT(result.Offset, point, index, point2, index2);
            Vector3 position = Type.GetInterpolatedPosition(point, Points[point.Index + 1], t);
            if (formation != null)
                return GetFormationPosition(position);

            return position;
        }

        private Vector3 GetFormationPosition(Vector3 position)
        {
            // TODO
            return position;
        }

        /// <summary>
        /// Get current rotation on the spline.
        /// </summary>
        public Vector3 GetRotation()
        {
            ISplineModeInterpolatedOffset result = Mode.GetInterpolatedOffset(Offset);
            CalculatePoints(result.Offset, out ISplinePoint point, out int index, out ISplinePoint point2, out int index2);

            float t = CalculateT(result.Offset, point, index, point2, index2);
            Vector3 rotation = Type.GetInterpolatedRotation(point, Points[point.Index + 1], t);

            Vector3 vector = Vector3.Normalize(rotation);
            if (Direction == SplineDirection.Backward)
                vector = -vector;

            float yaw = (float)Math.Atan2(-vector.X, -vector.Z);
            return new Vector3(yaw, 0.0f, 0.0f);
        }
    }
}
