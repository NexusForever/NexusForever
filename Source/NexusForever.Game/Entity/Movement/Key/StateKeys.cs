using NexusForever.Game.Static.Entity.Movement.Command.State;

namespace NexusForever.Game.Entity.Movement.Key
{
    public class StateKeys : Keys<StateFlags>
    {
        /// <summary>
        /// Calculate the interpolcated <see cref="StateFlags"/> value at the current time.
        /// </summary>
        public override StateFlags CalculateInterpolated(StateFlags v1, StateFlags v2, float t)
        {
            // no interpolation occurs, just return left side
            return v1;
        }
    }
}
