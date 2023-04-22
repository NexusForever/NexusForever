using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class SpellHandler
    {
        /// <summary>
        /// This handler is used when the player has enabled continous casting in Settings > Controls
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientCastSpellContinuous)]
        public static void HandleCastSpell(IWorldSession session, ClientCastSpellContinuous castSpell)
        {
            IItem item = session.Player.Inventory.GetItem(InventoryLocation.Ability, castSpell.BagIndex);
            if (item == null)
                throw new InvalidPacketValueException();

            ICharacterSpell characterSpell = session.Player.SpellManager.GetSpell(item.Id);
            if (characterSpell == null)
                throw new InvalidPacketValueException();

            characterSpell.Cast(castSpell.ButtonPressed);
        }

        [MessageHandler(GameMessageOpcode.ClientSpellStopCast)]
        public static void HandleSpellStopCast(IWorldSession session, ClientSpellStopCast spellStopCast)
        {
            // TODO: handle CastResult, client only sends SpellCancelled and SpellInterrupted
            session.Player.CancelSpellCast(spellStopCast.CastingId);
        }

        /// <summary>
        /// This Handler is ued when a player has disabled continuous casting in Settings > Controls
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientCastSpell)]
        public static void HandleCastSpell(IWorldSession session, ClientCastSpell castSpell)
        {
            IItem item = session.Player.Inventory.GetItem(InventoryLocation.Ability, castSpell.BagIndex);
            if (item == null)
                throw new InvalidPacketValueException();

            ICharacterSpell characterSpell = session.Player.SpellManager.GetSpell(item.Id);
            if (characterSpell == null)
                throw new InvalidPacketValueException();

            characterSpell.Cast();
        }

        [MessageHandler(GameMessageOpcode.ClientCancelEffect)]
        public static void HandlePlayerCastSpell(IWorldSession session, ClientCancelEffect cancelSpell)
        {
            //TODO: integrate into some Spell System removal queue & do the checks & handle stopped effects
            session.Player.EnqueueToVisible(new ServerSpellFinish
            {
                ServerUniqueId  = cancelSpell.ServerUniqueId
            },true);
        }

        [MessageHandler(GameMessageOpcode.ClientChangeActiveActionSet)]
        public static void HandleChangeActiveActionSet(IWorldSession session, ClientChangeActiveActionSet changeActiveActionSet)
        {
            session.EnqueueMessageEncrypted(new ServerChangeActiveActionSet
            {
                SpecError      = session.Player.SpellManager.SetActiveActionSet(changeActiveActionSet.ActionSetIndex),
                ActionSetIndex = session.Player.SpellManager.ActiveActionSet
            });

            session.Player.SpellManager.SendServerAbilityPoints();
        }

        [MessageHandler(GameMessageOpcode.ClientRequestActionSetChanges)]
        public static void HandleRequestActionSetChanges(IWorldSession session, ClientRequestActionSetChanges requestActionSetChanges)
        {
            // TODO: check for client validity, e.g. Level & Spell4TierRequirements

            IActionSet actionSet = session.Player.SpellManager.GetActionSet(requestActionSetChanges.ActionSetIndex);

            List<IActionSetShortcut> shortcuts = actionSet.Actions.ToList();
            for (UILocation i = 0; i < (UILocation)requestActionSetChanges.Actions.Count; i++)
            {
                IActionSetShortcut shortcut = actionSet.GetShortcut(i);
                if (shortcut != null)
                    actionSet.RemoveShortcut(i);

                uint spell4BaseId = requestActionSetChanges.Actions[(int)i];
                if (spell4BaseId != 0u)
                {
                    IActionSetShortcut existingShortcut = shortcuts.SingleOrDefault(s => s.ObjectId == spell4BaseId);
                    byte tier = existingShortcut?.Tier ?? 1;
                    actionSet.AddShortcut(i, ShortcutType.Spell, spell4BaseId, tier);
                }
            }

            foreach (ClientRequestActionSetChanges.ActionTier actionTier in requestActionSetChanges.ActionTiers)
                session.Player.SpellManager.UpdateSpell(actionTier.Action, actionTier.Tier, requestActionSetChanges.ActionSetIndex);

            session.EnqueueMessageEncrypted(actionSet.BuildServerActionSet());
            if (requestActionSetChanges.ActionTiers.Count > 0)
                session.Player.SpellManager.SendServerAbilityPoints();

            // only new AMP can be added with this packet, filter out existing ones
            List<ushort> newAmps = requestActionSetChanges.Amps
                .Except(actionSet.Amps
                    .Select(a => (ushort)a.Entry.Id))
                .ToList();

            if (newAmps.Count > 0)
            {
                foreach (ushort id in newAmps)
                    actionSet.AddAmp(id);

               session.EnqueueMessageEncrypted(actionSet.BuildServerAmpList());
            }
        }

        [MessageHandler(GameMessageOpcode.ClientRequestAmpReset)]
        public static void HandleRequestAmpReset(IWorldSession session, ClientRequestAmpReset requestAmpReset)
        {
            // TODO: check for client validity 
            // TODO: handle reset cost

            IActionSet actionSet = session.Player.SpellManager.GetActionSet(requestAmpReset.ActionSetIndex);
            actionSet.RemoveAmp(requestAmpReset.RespecType, requestAmpReset.Value);
            session.EnqueueMessageEncrypted(actionSet.BuildServerAmpList());
        }

        [MessageHandler(GameMessageOpcode.ClientNonSpellActionSetChanges)]
        public static void HandleNonSpellActionSetChanges(IWorldSession session, ClientNonSpellActionSetChanges requestActionSetChanges)
        {
            // TODO: validate the rest of the shortcut types when known

            switch (requestActionSetChanges.ShortcutType)
            {
                case ShortcutType.Item:
                {
                    if (GameTableManager.Instance.Item.GetEntry(requestActionSetChanges.ObjectId) == null)
                        throw new InvalidPacketValueException();
                    break;
                }
                case ShortcutType.Spell:
                    throw new InvalidPacketValueException();
                case ShortcutType.None:
                    // Removing Shortcut. Intentionally leaving blank.
                    break;
                case ShortcutType.Macro:
                    // Allowed. Macros seem to be stored client side only.
                    break;
                default:
                    throw new NotImplementedException();
            }

            IActionSet actionSet = session.Player.SpellManager.GetActionSet(requestActionSetChanges.Unknown);
            if (requestActionSetChanges.ObjectId == 0u)
                actionSet.RemoveShortcut(requestActionSetChanges.ActionBarIndex);
            else
                actionSet.AddShortcut(requestActionSetChanges.ActionBarIndex, requestActionSetChanges.ShortcutType, requestActionSetChanges.ObjectId, 0);
        }
    }
}
