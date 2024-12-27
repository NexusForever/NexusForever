using System;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientNonSpellActionSetChangesHandler : IMessageHandler<IWorldSession, ClientNonSpellActionSetChanges>
    {
        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;

        public ClientNonSpellActionSetChangesHandler(
            IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientNonSpellActionSetChanges requestActionSetChanges)
        {
            // TODO: validate the rest of the shortcut types when known

            switch (requestActionSetChanges.ShortcutType)
            {
                case ShortcutType.Item:
                {
                    if (gameTableManager.Item.GetEntry(requestActionSetChanges.ObjectId) == null)
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
