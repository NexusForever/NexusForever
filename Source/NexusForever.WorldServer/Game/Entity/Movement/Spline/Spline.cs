using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Movement.Spline.Implementation;
using NexusForever.WorldServer.Game.Entity.Movement.Spline.Static;

namespace NexusForever.WorldServer.Game.Entity.Movement.Spline
{
    public class Spline : IEnumerable<SplinePoint>
    {
        public float Length { get; private set; }

        protected readonly List<SplinePoint> points = new List<SplinePoint>();

        private SplineTypeBase type;
        private ISplineMode mode;

        /// <summary>
        /// Initialise a new single spline with supplied <see cref="SplineTypeBase"/> and <see cref="ISplineMode"/>.
        /// </summary>
        public void Initialise(ushort splineId, SplineTypeBase splineType, ISplineMode splineMode)
        {
            type = splineType;
            mode = splineMode;

            Spline2Entry splineEntry = GameTableManager.Instance.Spline2.GetEntry(splineId);
            if (splineEntry == null)
                throw new ArgumentException();

            foreach (Spline2NodeEntry node in GameTableManager.Instance.Spline2Node.Entries
                .Where(s => s.SplineId == splineId)
                .OrderBy(s => s.Ordinal))
            {
                // TODO
                if (node.Delay > 0f)
                    throw new NotImplementedException();

                var position = new Vector3(node.Position0, node.Position1, node.Position2);
                points.Add(new SplinePoint(position)
                {
                    Length = node.FrameTime
                });
            }

            splineType.Initialise((uint)points.Count);

            Length = splineType.CalculateLengths(points);
        }

        /// <summary>
        /// Initialise a new custom spline with supplied <see cref="SplineTypeBase"/> and <see cref="ISplineMode"/>.
        /// </summary>
        public void Initialise(List<Vector3> nodes, SplineTypeBase splineType, ISplineMode splineMode)
        {
            type = splineType;
            mode = splineMode;

            foreach (Vector3 position in nodes)
                points.Add(new SplinePoint(position));

            splineType.Initialise((uint)points.Count);

            Length = splineType.CalculateLengths(points);
        }

        /// <summary>
        /// Get the final point position in the supplied <see cref="SplineDirection"/>.
        /// </summary>
        public Vector3 GetFinalPoint(SplineDirection direction)
        {
            SplinePoint p = points[(int)(direction == SplineDirection.Forward ? type.TopIndex + 1 : type.TopReverseIndex - 1)];
            return p.Position;
        }

        /// <summary>
        /// Get the <see cref="SplineDirection"/> and index of the next point from the specified <see cref="SplineDirection"/> and index.
        /// </summary>
        public (SplineDirection Direction, uint Point)? GetNextPoint(SplineDirection direction, uint point)
        {
            return mode.GetNextPoint(type, direction, point);
        }

        /// <summary>
        /// Get the length of the next segment from the specified <see cref="SplineDirection"/> and index.
        /// </summary>
        public float GetNextLength(SplineDirection direction, uint point)
        {
            SplinePoint p = points[(int)(direction == SplineDirection.Forward ? point : point - 1)];
            return p.Length;
        }

        /// <summary>
        /// Get the interpolated <see cref="Vector3"/> position in the specified <see cref="SplineDirection"/> at p.
        /// </summary>
        public Vector3 GetPosition(SplineDirection direction, float p)
        {
            return type.GetInterpolatedPoint(direction, p, points);
        }

        public IEnumerator<SplinePoint> GetEnumerator()
        {
            return points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
