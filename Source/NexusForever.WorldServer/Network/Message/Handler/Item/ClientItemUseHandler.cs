using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Handler.Item
{
    public class ClientItemUseHandler : IMessageHandler<IWorldSession, ClientItemUse>
    {
        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;
        private readonly IPrerequisiteManager prerequisiteManager;

        public ClientItemUseHandler(
            IGameTableManager gameTableManager,
            IPrerequisiteManager prerequisiteManager)
        {
            this.gameTableManager    = gameTableManager;
            this.prerequisiteManager = prerequisiteManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientItemUse itemUse)
        {
            // Before it sent this message the client created a pending cast of its own and stamped it
            // with itemUse.CastingId. It has no timeout for one, so every refusal below has to answer
            // that id or the ability stays greyed out by the predicted cooldown for the rest of the
            // session. A refusal that carries no spell id still clears the pending cast and releases
            // the cooldown, but shows the player no text.
            IItem item = session.Player.Inventory.GetItem(itemUse.Location);
            if (item == null)
            {
                // An honest client reaches this: a double click, or a use of an item that another path
                // moved or consumed between the click and this message. InvalidPacketValueException
                // force-disconnects the session, which is the wrong answer to a race.
                session.Player.SendSpellCastResult(itemUse.CastingId, 0, CastResult.ItemUnknown);
                return;
            }

            ItemSpecialEntry itemSpecial = gameTableManager.ItemSpecial.GetEntry(item.Info.Entry.ItemSpecialId00);
            if (itemSpecial == null)
            {
                // Also honest-client reachable: any item whose ItemSpecialId00 is 0 or names a row this
                // server's tables do not have. The client cannot tell a usable item from an unusable one.
                session.Player.SendSpellCastResult(itemUse.CastingId, 0, CastResult.ItemBad);
                return;
            }

            if (itemSpecial.Spell4IdOnActivate > 0u)
            {
                if (itemSpecial.PrerequisiteIdGeneric00 > 0 && !prerequisiteManager.Meets(session.Player, itemSpecial.PrerequisiteIdGeneric00))
                {
                    // GenericError.UnlockItemFailed (81) has no string in the client and renders the
                    // literal "(Unlocalized) Unhandled EGenericError Code (81)", and a generic error
                    // clears no pending cast. The cast result does both jobs, so it replaces it.
                    session.Player.SendSpellCastResult(itemUse.CastingId, itemSpecial.Spell4IdOnActivate,
                        CastResult.SpellPreRequisites);
                    return;
                }

                if (!session.Player.Inventory.ItemUse(item))
                {
                    // ItemUse only returns false when there is nothing left to consume: no charges on a
                    // charged item, or an empty stack.
                    session.Player.SendSpellCastResult(itemUse.CastingId, itemSpecial.Spell4IdOnActivate,
                        CastResult.ItemCharges);
                    return;
                }

                session.Player.CastSpell(itemSpecial.Spell4IdOnActivate, new SpellParameters
                {
                    PrimaryTargetId         = itemUse.TargetUnitId,
                    TargetPosition          = itemUse.Position,
                    // ClientItemUse.CastingId is the client's own cast id, not a server casting id
                    ClientSpellCastUniqueId = itemUse.CastingId
                });
            }
            else
                session.Player.SendSpellCastResult(itemUse.CastingId, 0, CastResult.ItemNoSpellId);
        }
    }
}
