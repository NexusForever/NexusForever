using NexusForever.Game.Abstract.Entity;
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

        /// <summary>
        /// Invoked when this entity is interacted with.
        /// </summary>
        void OnInteract(IPlayer activator)
        {
        }

        /// <summary>
        /// Invoked when this entity receives a successful activation.
        /// </summary>
        void OnActivateSuccess(IPlayer activator)
        {
        }

        /// <summary>
        /// Invoked when this entity receives a failed activation.
        /// </summary>
        void OnActivateFail(IPlayer activator)
        {
        }
    }
}
