using System.Collections.Generic;
using System.Numerics;
using NexusForever.WorldServer.Game.Entity.Movement.Spline.Static;

namespace NexusForever.WorldServer.Game.Entity.Movement.Spline.Implementation
{
    public abstract class SplineTypeBase
    {
        public uint BottomIndex { get; protected set; }
        public uint TopIndex { get; protected set; }
        public uint BottomReverseIndex { get; protected set; }
        public uint TopReverseIndex { get; protected set; }

        public abstract void Initialise(uint points);

        /// <summary>
        /// Get the interpolated <see cref="Vector3"/> between supplied points in <see cref="SplineDirection"/> at normalised (0 - totalLength) position p.
        /// </summary>
        public abstract Vector3 GetInterpolatedPoint(SplineDirection direction, float p, List<SplinePoint> points);

        /// <summary>
        /// Get the interpolated <see cref="Vector3"/> between supplied points at normalised (0-1) distance t.
        /// </summary>
        public abstract Vector3 GetInterpolatedPoint(float t, params SplinePoint[] points);

        /// <summary>
        /// Calculate the length of each segment and returns the total length.
        /// </summary>
        public abstract float CalculateLengths(List<SplinePoint> points);
    }
}
