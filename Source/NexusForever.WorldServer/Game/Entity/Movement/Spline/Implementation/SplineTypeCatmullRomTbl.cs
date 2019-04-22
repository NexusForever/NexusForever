using System.Collections.Generic;

namespace NexusForever.WorldServer.Game.Entity.Movement.Spline.Implementation
{
    public class SplineTypeCatmullRomTbl : SplineTypeCatmullRom
    {
        public override float CalculateLengths(List<SplinePoint> points)
        {
            float totalLength = 0f;
            for (var i = 1; i < points.Count - 2; i++)
            {
                SplinePoint p0 = points[i];
                SplinePoint p1 = points[i + 1];

                // no need to calculate segment length from interpolated points, tbl is nice enough to provide them
                float length = p1.Length - p0.Length;
                p1.Length = length;
                totalLength += length;
            }

            return totalLength;
        }
    }
}
