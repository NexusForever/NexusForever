using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Game.Spell.Static;
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

            // true is probably "begin casting"
            if (!castSpell.Unknown48)
                return;

            session.Player.CastSpell(new SpellParameters
            {
                SpellInfo = spell.Info.GetSpellInfo(spell.Tier)
            });
        }

        [MessageHandler(GameMessageOpcode.ClientChangeActiveActionSet)]
        public static void HandleClientChangeActionSet(WorldSession session, ClientChangeActiveActionSet clientChangeActiveActionSet)
        {
            session.EnqueueMessageEncrypted(new ServerChangeActiveActionSet
            {
                ActionSetError = session.Player.SpellManager.SetActiveActionSet(clientChangeActiveActionSet.ActionSetIndex),
                ActionSetIndex = session.Player.SpellManager.activeActionSet
            });
            session.Player.SpellManager.SendServerAbilityPoints();
        }

        [MessageHandler(GameMessageOpcode.ClientUpdateActionSet)]
        public static void HandleClientChangeActionSet(WorldSession session, ClientUpdateActionSet clientUpdateActionSet)
        {
            // TODO: check for client validity, e.g. Level & Spell4TierRequirements
            
            for (byte i = 0; i < clientUpdateActionSet.Actions.Count; i++)
            {
                ActionSetAction action = session.Player.SpellManager.GetSpellFromActionSet(clientUpdateActionSet.ActionSetIndex,  (UILocation)i);
                if (action == null || action.ObjectId != clientUpdateActionSet.Actions[i])
                {
                    session.Player.SpellManager.RemoveSpellFromActionSet(clientUpdateActionSet.ActionSetIndex, (UILocation)i);
                    if (clientUpdateActionSet.Actions[i] > 0)
                        session.Player.SpellManager.AddSpellToActionSet(clientUpdateActionSet.ActionSetIndex, clientUpdateActionSet.Actions[i], (UILocation)i);
                }
            }

            for (byte i = 0; i < clientUpdateActionSet.ActionTiers.Count; i++)
            {
                var (spell, tier) = clientUpdateActionSet.ActionTiers[i];
                if (spell > 0)
                    session.Player.SpellManager.UpdateActionSetSpellTier(clientUpdateActionSet.ActionSetIndex, spell, tier);
            }

            session.Player.SpellManager.SendServerSpellList();
            session.Player.SpellManager.SendServerActionSet(clientUpdateActionSet.ActionSetIndex);

            if (clientUpdateActionSet.ActionTiers.Count > 0)
            {
                session.Player.SpellManager.SendServerAbilityPoints();
            }

            if (clientUpdateActionSet.AMPs.Count > 0)
            {
                session.Player.SpellManager.UpdateActionSetAMPs(clientUpdateActionSet.ActionSetIndex, clientUpdateActionSet.AMPs);
                session.Player.SpellManager.SendServerAMPList(clientUpdateActionSet.ActionSetIndex);
            }


        }
        //[MessageHandler(GameMessageOpcode.ClientChangeInnate)]

        [MessageHandler(GameMessageOpcode.ClientRequestAMPReset)]
        public static void HandleClientChangeActionSet(WorldSession session, ClientRequestAMPReset clientRequestAMPReset)
        {
            // TODO: check for client validity,
            // TODO: handle reset cost
            session.Player.SpellManager.UpdateActionSetAMPs(clientRequestAMPReset.ActionSetIndex, new List<ushort>());
            session.Player.SpellManager.SendServerAMPList(clientRequestAMPReset.ActionSetIndex);
        }

    }
}
