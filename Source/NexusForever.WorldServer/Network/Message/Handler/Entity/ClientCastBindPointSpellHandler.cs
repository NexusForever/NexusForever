using NexusForever.Game.Abstract.Entity;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Spell;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity
{
    /// <summary>
    /// Sent when the player uses a transmat terminal (bind point) to bind there.
    /// </summary>
    public class ClientCastBindPointSpellHandler : IMessageHandler<IWorldSession, ClientCastBindPointSpell>
    {
        public void HandleMessage(IWorldSession session, ClientCastBindPointSpell castBindPointSpell)
        {
            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(castBindPointSpell.UnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            uint bindPointId = entity.CreatureEntry?.BindPointId ?? 0u;
            if (bindPointId == 0u)
                throw new InvalidPacketValueException();

            // TODO: sanity check for range etc.

            session.Player.SetBindPoint((ushort)bindPointId, (ushort)session.Player.Map.Entry.Id, session.Player.Position);
        }
    }
}
