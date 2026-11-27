using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;

namespace NexusForever.Script.Template
{
    public interface IWorldEntityScript : IGridEntityScript
    {
        /// <summary>
        /// Invoked when a player chooses to talk to the entity.
        /// </summary>
        void OnPlayerInteract(IPlayer player)
        {
        }

        /// <summary>
        /// Invoked when a visible player directs an emote at the entity or nearby.
        /// </summary>
        void OnPlayerEmote(IPlayer player, ushort emoteId)
        {
        }

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
