using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class SpellHandler
    {
        [MessageHandler(GameMessageOpcode.ClientCastSpell)]
        public static void HandlePlayerCastSpell(WorldSession session, ClientCastSpell castSpell)
        {
            Item item = session.Player.Inventory.GetItem(InventoryLocation.Ability, castSpell.BagIndex);
            if (item == null)
                throw new InvalidPacketValueException();

            UnlockedSpell spell = session.Player.SpellManager.GetSpell(item.Id);
            if (spell == null)
                throw new InvalidPacketValueException();

            // true is button pressed, false is sent on release
            if (!castSpell.ButtonPressed)
                return;

            session.Player.CastSpell(new SpellParameters
            {
                SpellInfo = spell.Info.GetSpellInfo(spell.Tier)
            });
        }

        [MessageHandler(GameMessageOpcode.Client009A)]
        public static void HandlePlayerCastSpell(WorldSession session, Client009A castSpell)
        {
            Item item = session.Player.Inventory.GetItem(InventoryLocation.Ability, castSpell.BagIndex);
            if (item == null)
                throw new InvalidPacketValueException();

            UnlockedSpell spell = session.Player.SpellManager.GetSpell(item.Id);
            if (spell == null)
                throw new InvalidPacketValueException();

            session.Player.CastSpell(new SpellParameters
            {
                SpellInfo = spell.Info.GetSpellInfo(spell.Tier)
            });
        }

        [MessageHandler(GameMessageOpcode.ClientCancelEffect)]
        public static void HandlePlayerCastSpell(WorldSession session, ClientCancelEffect cancelSpell)
        {
            //TODO: integrate into some Spell System removal queue & do the checks & handle stopped effects
            session.Player.EnqueueToVisible(new ServerSpellFinish
            {
                ServerUniqueId  = cancelSpell.ServerUniqueId
            },true);
        }
    }
}
