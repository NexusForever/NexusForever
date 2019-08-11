using System;
using System.Collections.Generic;
using System.Numerics;
using NexusForever.Shared;
using NexusForever.WorldServer.Game.Map;

namespace NexusForever.WorldServer.Game.Entity.Movement.Generator
{
    public class RandomMovementGenerator : IMovementGenerator
    {
        private const float StepSize = 2f;

        public Vector3 Begin { get; set; }
        public Vector3 Leash { get; set; }
        public float Range { get; set; }
        public BaseMap Map { get; set; }

        public List<Vector3> CalculatePath()
        {
            var points = new List<Vector3> { Begin };

            Vector3 final = Leash.GetRandomPoint2D(Range);
            final.Y = Map.GetTerrainHeight(final.X, final.Z);

            float angle = MathF.Atan2(final.Z - Begin.Z, final.X - Begin.X);
            for (int i = 0; i < MathF.Floor(Vector3.Distance(Begin, final) / StepSize); i++)
            {
                Vector3 next = points[points.Count - 1].GetPoint2D(angle, StepSize);
                next.Y = Map.GetTerrainHeight(next.X, next.Z);
                points.Add(next);
            }

            points.Add(final);
            return points;
        }
    }
}
