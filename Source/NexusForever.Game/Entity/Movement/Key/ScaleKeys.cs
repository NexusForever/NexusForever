namespace NexusForever.Game.Entity.Movement.Key
{
    public class ScaleKeys : Keys<float>
    {
        /// <summary>
        /// Calculate the interpolcated <see cref="float"/> scale value at the current time.
        /// </summary>
        public override float CalculateInterpolated(float v1, float v2, float t)
        {
            return MathsExtensions.Lerp(v1, v2, t);
        }
    }
}
