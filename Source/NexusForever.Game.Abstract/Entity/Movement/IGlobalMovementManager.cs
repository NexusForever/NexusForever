using NexusForever.Game.Abstract.Entity.Movement.Spline;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Abstract.Entity.Movement
{
    public interface IGlobalMovementManager
    {
        void Initialise();

        /// <summary>
        /// Return a new <see cref="ISplineMode"/> for supplied <see cref="SplineMode"/>.
        /// </summary>
        ISplineMode NewSplineMode(SplineMode mode);
    }
}