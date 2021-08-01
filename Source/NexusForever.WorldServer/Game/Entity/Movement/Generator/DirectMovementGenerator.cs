using System;
using System.Collections.Generic;
using System.Numerics;
using NexusForever.Shared;
using NexusForever.WorldServer.Game.Map;

namespace NexusForever.WorldServer.Game.Entity.Movement.Generator
{
    public class DirectMovementGenerator : IMovementGenerator
    {
        private const float StepSize = 2f;

        public Vector3 Begin { get; set; }
        public Vector3 Final { get; set; }
        public BaseMap Map { get; set; }

        public List<Vector3> CalculatePath()
        {
            var points = new List<Vector3> { Begin };

            float angle = MathF.Atan2(Final.Z - Begin.Z, Final.X - Begin.X);
            for (int i = 0; i < MathF.Floor(Vector3.Distance(Begin, Final) / StepSize); i++)
            {
                Vector3 next = points[points.Count - 1].GetPoint2D(angle, StepSize);
                next.Y = Map.GetTerrainHeight(next.X, next.Z);
                points.Add(next);
            }

            Vector3 final = Final;
            final.Y = Map.GetTerrainHeight(Final.X, Final.Z);
            points.Add(final);

            return points;
        }
    }
}
