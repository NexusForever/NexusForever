using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Generator;
using NexusForever.Game.Abstract.Map;

namespace NexusForever.Game.Entity.Movement.Generator
{
    public class DirectMovementGenerator : IDirectMovementGenerator
    {
        private const float StepSize = 2f;

        public Vector3 Begin { get; set; }
        public Vector3 Final { get; set; }
        public IBaseMap Map { get; set; }

        public List<Vector3> CalculatePath()
        {
            var points = new List<Vector3> { Begin };

            float angle = MathF.Atan2(Final.Z - Begin.Z, Final.X - Begin.X);
            for (int i = 0; i < MathF.Floor(Vector3.Distance(Begin, Final) / StepSize); i++)
            {
                Vector3 next = points[points.Count - 1].GetPoint2D(angle, StepSize);
                next.Y = Map?.GetTerrainHeight(next.X, next.Z) ?? 0f;
                points.Add(next);
            }

            points.Add(Final);
            return points;
        }
    }
}
