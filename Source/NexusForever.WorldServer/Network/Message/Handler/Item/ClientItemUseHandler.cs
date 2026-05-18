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
            IItem item = session.Player.Inventory.GetItem(itemUse.Location);
            if (item == null)
                throw new InvalidPacketValueException();

            ItemSpecialEntry itemSpecial = gameTableManager.ItemSpecial.GetEntry(item.Info.Entry.ItemSpecialId00);
            if (itemSpecial == null)
                throw new InvalidPacketValueException();

            if (itemSpecial.Spell4IdOnActivate > 0u)
            {
                if (itemSpecial.PrerequisiteIdGeneric00 > 0 && !prerequisiteManager.Meets(session.Player, itemSpecial.PrerequisiteIdGeneric00))
                {
                    session.Player.SendGenericError(GenericError.UnlockItemFailed); // TODO: Confirm right error message.
                    return;
                }

                if (session.Player.Inventory.ItemUse(item))
                {
                    session.Player.CastSpell(itemSpecial.Spell4IdOnActivate, new SpellParameters
                    {
                        PrimaryTargetId = itemUse.TargetUnitId,
                        Position        = itemUse.Position
                    });
                }
            }
        }
    }
}
