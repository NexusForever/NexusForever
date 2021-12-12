using NexusForever.Game.Abstract.Entity;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity
{
    /// <remarks>
    /// Possibly only used by Bindpoint entities
    /// </remarks>
    public class ClientActivateUnitInteractionHandler : IMessageHandler<IWorldSession, ClientActivateUnitInteraction>
    {
        public void HandleMessage(IWorldSession session, ClientActivateUnitInteraction activateUnitInteraction)
        {
            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(activateUnitInteraction.ActivateUnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            entity.OnActivateCast(session.Player, activateUnitInteraction.ClientUniqueId);
        }
    }
}
