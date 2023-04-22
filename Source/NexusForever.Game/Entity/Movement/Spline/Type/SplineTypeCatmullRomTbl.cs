using NexusForever.Game.Abstract.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Type
{
    public class SplineTypeCatmullRomTbl : SplineTypeCatmullRom
    {
        public override float CalculateLengths(List<ISplinePoint> points)
        {
            float totalLength = 0f;
            for (int i = (int)BottomIndex; i <= TopIndex; i++)
            {
                ISplinePoint p0 = points[i];
                ISplinePoint p1 = points[i + 1];

                // no need to calculate segment length from interpolated points, tbl is nice enough to provide them
                float length = p1.Length - p0.Length;
                p0.Length = length;
                totalLength += length;
            }

            return totalLength;
        }
    }
}
