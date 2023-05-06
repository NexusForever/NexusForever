using NexusForever.Game.Abstract.Entity.Movement.Spline;

namespace NexusForever.Script.Template
{
    public interface IWorldEntityScript : IGridEntityScript
    {
        
        /// <summary>
        /// Invoked when <see cref="ISplinePath"/> is stopped.
        /// </summary>
        void OnSplineStop()
        {
        }

        /// <summary>
        /// Invoked when a <see cref="ISplinePath"/> point is reached.
        /// </summary>
        void OnSplinePoint(uint point)
        {
        }
    }
}
