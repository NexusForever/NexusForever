using NexusForever.Game.Static.Entity.Movement.Command.Mode;

namespace NexusForever.Game.Entity.Movement.Key
{
    public class ModeKeys : Keys<ModeType>
    {
        /// <summary>
        /// Calculate the interpolcated <see cref="ModeType"/> value at the current time.
        /// </summary>
        public override ModeType CalculateInterpolated(ModeType v1, ModeType v2, float t)
        {
            // no interpolation occurs, just return left side
            return v1;
        }
    }
}
