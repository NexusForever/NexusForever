using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Template;
using NexusForever.Game.Static.Entity.Movement.Spline;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Entity.Movement.Spline.Template
{
    public class SplineTemplateTbl : ISplineTemplateTbl
    {
        public SplineType Type { get; private set; }
        public List<ISplineTemplatePoint> Points { get; } = new();

        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;

        public SplineTemplateTbl(
            IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        #endregion

        /// <summary>
        /// Initialise spline template with spline id.
        /// </summary>
        public void Initialise(ushort splineId)
        {
            Spline2Entry splineEntry = gameTableManager.Spline2.GetEntry(splineId);
            if (splineEntry == null)
                throw new ArgumentOutOfRangeException();

            Type = splineEntry.SplineType;

            foreach (Spline2NodeEntry nodeEntry in gameTableManager.Spline2Node.Entries
                .Where(s => s.SplineId == splineId)
                .OrderBy(s => s.Ordinal))
            {
                Points.Add(new SplineTemplatePoint
                {
                    Position  = new Vector3(nodeEntry.Position0, nodeEntry.Position1, nodeEntry.Position2),
                    Rotation  = new Quaternion(nodeEntry.Facing0, nodeEntry.Facing1, nodeEntry.Facing2, nodeEntry.Facing3),
                    Delay     = nodeEntry.Delay > 0 ? nodeEntry.Delay : null,
                    FrameTime = nodeEntry.FrameTime
                });
            }
        }
    }
}
