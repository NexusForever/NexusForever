using NexusForever.Game.Abstract.Entity;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Spell;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity
{
    /// <remarks>
    /// Possibly only used by Bindpoint entities
    /// </remarks>
    public class ClientCastBindPointSpellHandler : IMessageHandler<IWorldSession, ClientCastBindPointSpell>
    {
        public void HandleMessage(IWorldSession session, ClientCastBindPointSpell activateUnitInteraction)
        {
            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(activateUnitInteraction.UnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            entity.OnActivateCast(session.Player, activateUnitInteraction.UnitId);
        }
    }
}
