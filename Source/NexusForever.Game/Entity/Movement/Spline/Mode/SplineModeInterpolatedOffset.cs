using NexusForever.Game.Abstract.Entity.Movement.Spline.Mode;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Mode
{
    public class SplineModeInterpolatedOffset : ISplineModeInterpolatedOffset
    {
        public float Offset { get; set; }
        public SplineDirection Direction { get; set; }
        public bool Finalised { get; set; }
    }
}
