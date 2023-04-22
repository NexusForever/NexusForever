﻿using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Spline;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Type;
using NexusForever.Game.Entity.Movement.Spline.Type;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline
{
    public class SplinePath : ISplinePath
    {
        public SplineType Type { get; }
        public SplineMode Mode { get; }
        public float Speed { get; }
        public float Length => spline.Length;
        public float Position { get; private set; }
        public bool IsFinialised { get; private set; }

        private readonly ISpline spline;

        private uint point;
        private SplineDirection direction;
        private float remaining;

        /// <summary>
        /// Create a new single spline <see cref="ISplinePath"/> with supplied <see cref="SplineMode"/> and speed.
        /// </summary>
        public SplinePath(ushort splineId, SplineMode mode, float speed)
        {
            Type     = SplineType.CatmullRom;
            Mode     = mode;
            Speed    = speed;

            ISplineType splineType = new SplineTypeCatmullRomTbl();

            ISplineMode splineMode = GlobalMovementManager.Instance.NewSplineMode(mode);
            if (splineMode == null)
                throw new ArgumentOutOfRangeException();

            spline = new Spline();
            spline.Initialise(splineId, splineType, splineMode);

            direction = splineMode.InitialDirection;
            point     = direction == SplineDirection.Forward ? splineType.BottomIndex : splineType.BottomReverseIndex;
            remaining = spline.GetNextLength(direction, point);
        }

        /// <summary>
        /// Create a new custom spline <see cref="ISplinePath"/> with supplied <see cref="SplineType"/>, <see cref="SplineMode"/> and speed.
        /// </summary>
        public SplinePath(List<Vector3> nodes, SplineType type, SplineMode mode, float speed)
        {
            Type  = type;
            Mode  = mode;
            Speed = speed;

            ISplineType splineType;
            switch (type)
            {
                case SplineType.Linear:
                    splineType = new SplineTypeLinear();
                    break;
                case SplineType.CatmullRom:
                    splineType = new SplineTypeCatmullRom();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ISplineMode splineMode = GlobalMovementManager.Instance.NewSplineMode(mode);
            if (splineMode == null)
                throw new ArgumentException();

            spline = new Spline();
            spline.Initialise(nodes, splineType, splineMode);

            direction = splineMode.InitialDirection;
            point     = direction == SplineDirection.Forward ? splineType.BottomIndex : splineType.BottomReverseIndex;
            remaining = spline.GetNextLength(direction, point);
        }

        public void Update(double lastTick)
        {
            if (IsFinialised)
                return;

            float delta = (float)(lastTick * Speed);
            Position += delta;

            while (delta > 0f)
            {
                float distance = MathF.Min(delta, remaining);
                delta -= distance;

                if (distance >= remaining)
                {
                    if (!GetNextPoint())
                    {
                        IsFinialised = true;
                        break;
                    }
                }
                else
                    remaining -= distance;
            }
        }

        /// <summary>
        /// Get current <see cref="Vector3"/> position on spline.
        /// </summary>
        public Vector3 GetPosition()
        {
            if (IsFinialised)
                return spline.GetFinalPoint(direction);

            float p = Position % spline.Length;
            return spline.GetPosition(direction, p);
        }

        /// <summary>
        /// Gets the next spline point, if false is returned the spline has finished.
        /// </summary>
        private bool GetNextPoint()
        {
            (SplineDirection Direction, uint Point)? nextPoint = spline.GetNextPoint(direction, point);
            if (nextPoint == null)
                return false;

            direction = nextPoint.Value.Direction;
            point     = nextPoint.Value.Point;
            remaining = spline.GetNextLength(direction, point);
            return true;
        }
    }
}
