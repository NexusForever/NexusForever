using NexusForever.Game.Abstract.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline
{
    public class SplinePointLength : ISplinePointLength
    {
        public float Distance { get; set; }
        public float T { get; set; }
        public float Delay { get; set; }
    }
}
