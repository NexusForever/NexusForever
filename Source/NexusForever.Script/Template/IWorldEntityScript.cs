using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;

namespace NexusForever.Script.Template
{
    public interface IWorldEntityScript : IGridEntityScript
    {
        /// <summary>
        /// Invoked when <see cref="IPositionCommand"/> is finalised.
        /// </summary>
        void OnPositionEntityCommandFinalise(IPositionCommand command)
        {
        }

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> enters a zone.
        /// </summary>
        void OnEnterZone(IWorldEntity entity, uint zone)
        {
        }
    }
}
