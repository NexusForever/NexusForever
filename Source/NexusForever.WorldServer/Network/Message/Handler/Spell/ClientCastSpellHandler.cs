using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientCastSpellHandler : IMessageHandler<IWorldSession, ClientCastSpell>
    {
        /// <summary>
        /// This Handler is ued when a player has disabled continuous casting in Settings > Controls
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientCastSpell castSpell)
        {
            IItem item = session.Player.Inventory.GetItem(InventoryLocation.Ability, castSpell.BagIndex);
            if (item == null)
                throw new InvalidPacketValueException();

            ICharacterSpell characterSpell = session.Player.SpellManager.GetSpell(item.Id);
            if (characterSpell == null)
                throw new InvalidPacketValueException();

            characterSpell.Cast();
        }
    }
}
