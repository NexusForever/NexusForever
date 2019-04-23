using System.Collections.Generic;

namespace NexusForever.WorldServer.Game.Entity.Movement.Spline.Implementation
{
    public class SplineTypeCatmullRomTbl : SplineTypeCatmullRom
    {
        public override float CalculateLengths(List<SplinePoint> points)
        {
            float totalLength = 0f;
            for (int i = (int)BottomIndex; i <= TopIndex; i++)
            {
                SplinePoint p0 = points[i];
                SplinePoint p1 = points[i + 1];

                // no need to calculate segment length from interpolated points, tbl is nice enough to provide them
                float length = p1.Length - p0.Length;
                p0.Length = length;
                totalLength += length;
            }

            return totalLength;
        }
    }
}
