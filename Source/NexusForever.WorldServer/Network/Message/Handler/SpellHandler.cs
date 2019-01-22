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

        [MessageHandler(GameMessageOpcode.ClientRequestActionSetChanges)]
        public static void HandleClientChangeActionSet(WorldSession session, ClientRequestActionSetChanges clientRequestActionSetChanges)
        {
            // TODO: check for client validity, e.g. Level & Spell4TierRequirements
            
            for (byte i = 0; i < clientRequestActionSetChanges.Actions.Count; i++)
            {
                ActionSetAction action = session.Player.SpellManager.GetSpellFromActionSet(clientRequestActionSetChanges.ActionSetIndex,  (UILocation)i);
                if (action == null || action.ObjectId != clientRequestActionSetChanges.Actions[i])
                {
                    session.Player.SpellManager.RemoveSpellFromActionSet(clientRequestActionSetChanges.ActionSetIndex, (UILocation)i);
                    if (clientRequestActionSetChanges.Actions[i] > 0)
                        session.Player.SpellManager.AddSpellToActionSet(clientRequestActionSetChanges.ActionSetIndex, clientRequestActionSetChanges.Actions[i], (UILocation)i);
                }
            }

            for (byte i = 0; i < clientRequestActionSetChanges.ActionTiers.Count; i++)
            {
                var (spell, tier) = clientRequestActionSetChanges.ActionTiers[i];
                if (spell > 0)
                    session.Player.SpellManager.UpdateActionSetSpellTier(clientRequestActionSetChanges.ActionSetIndex, spell, tier);
            }

            session.Player.SpellManager.SendServerSpellList();
            session.Player.SpellManager.SendServerActionSet(clientRequestActionSetChanges.ActionSetIndex);

            if (clientRequestActionSetChanges.ActionTiers.Count > 0)
            {
                session.Player.SpellManager.SendServerAbilityPoints();
            }

            if (clientRequestActionSetChanges.AMPs.Count > 0)
            {
                session.Player.SpellManager.UpdateActionSetAMPs(clientRequestActionSetChanges.ActionSetIndex, clientRequestActionSetChanges.AMPs);
                session.Player.SpellManager.SendServerAMPList(clientRequestActionSetChanges.ActionSetIndex);
            }


        }
        //[MessageHandler(GameMessageOpcode.ClientChangeInnate)]

        [MessageHandler(GameMessageOpcode.ClientRequestAMPReset)]
        public static void HandleClientChangeActionSet(WorldSession session, ClientRequestAMPReset clientRequestAMPReset)
        {
            // TODO: check for client validity 
            // TODO: handle reset cost

            if (clientRequestAMPReset.ResetType == 1)
                session.Player.SpellManager.ResetActionSetAMPCategory(clientRequestAMPReset.ActionSetIndex, clientRequestAMPReset.Value);
            else if (clientRequestAMPReset.ResetType == 2)
                session.Player.SpellManager.RemoveActionSetAMP(clientRequestAMPReset.ActionSetIndex, clientRequestAMPReset.Value);
            else
                session.Player.SpellManager.UpdateActionSetAMPs(clientRequestAMPReset.ActionSetIndex, new List<ushort>());

            session.Player.SpellManager.SendServerAMPList(clientRequestAMPReset.ActionSetIndex);
        }
    }
}
