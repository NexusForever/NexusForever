using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class ItemHandler
    {
        [MessageHandler(GameMessageOpcode.ClientItemMove)]
        public static void HandleItemMove(IWorldSession session, ClientItemMove itemMove)
        {
            IItem item = session.Player.Inventory.GetItem(itemMove.From);
            if (item == null)
                throw new InvalidPacketValueException();

            GenericError? result = session.Player.Inventory.CanMoveItem(item, itemMove.To);
            if (result.HasValue)
            {
                session.EnqueueMessageEncrypted(new ServerItemError
                {
                    ItemGuid  = item.Guid,
                    ErrorCode = result.Value
                });
                return;
            }

            session.Player.Inventory.ItemMove(item, itemMove.To);
        }

        [MessageHandler(GameMessageOpcode.ClientItemSplit)]
        public static void HandleItemSplit(IWorldSession session, ClientItemSplit itemSplit)
        {
            session.Player.Inventory.ItemSplit(itemSplit.Guid, itemSplit.Location, itemSplit.Count);
        }

        [MessageHandler(GameMessageOpcode.ClientItemDelete)]
        public static void HandleItemDelete(IWorldSession session, ClientItemDelete itemDelete)
        {
            session.Player.Inventory.ItemDelete(itemDelete.From);
        }

        [MessageHandler(GameMessageOpcode.ClientItemUse)]
        public static void HandleItemUse(IWorldSession session, ClientItemUse itemUse)
        {
            IItem item = session.Player.Inventory.GetItem(itemUse.Location);
            if (item == null)
                throw new InvalidPacketValueException();

            ItemSpecialEntry itemSpecial = GameTableManager.Instance.ItemSpecial.GetEntry(item.Info.Entry.ItemSpecialId00);
            if (itemSpecial == null)
                throw new InvalidPacketValueException();

            if (itemSpecial.Spell4IdOnActivate > 0u)
            {
                if (itemSpecial.PrerequisiteIdGeneric00 > 0 && !PrerequisiteManager.Instance.Meets(session.Player, itemSpecial.PrerequisiteIdGeneric00))
                {
                    session.Player.SendGenericError(GenericError.UnlockItemFailed); // TODO: Confirm right error message.
                    return;
                }

                if (session.Player.Inventory.ItemUse(item))
                {
                    session.Player.CastSpell(itemSpecial.Spell4IdOnActivate, new SpellParameters
                    {
                        PrimaryTargetId = itemUse.TargetUnitId,
                        TargetPosition  = itemUse.Position
                    });
                }
            }
        }

        [MessageHandler(GameMessageOpcode.ClientItemGenericUnlock)]
        public static void HandleItemGenericUnlock(IWorldSession session, ClientItemGenericUnlock itemGenericUnlock)
        {
            IItem item = session.Player.Inventory.GetItem(itemGenericUnlock.Location);
            if (item == null)
                throw new InvalidPacketValueException();

            GenericUnlockEntryEntry entry = GameTableManager.Instance.GenericUnlockEntry.GetEntry(item.Info.Entry.GenericUnlockSetId);
            if (entry == null)
                throw new InvalidPacketValueException();

            // TODO: should some client error be shown for this?
            if (!session.Account.GenericUnlockManager.IsUnlocked((GenericUnlockType)entry.GenericUnlockTypeEnum, entry.UnlockObject))
                return;

            if (session.Player.Inventory.ItemUse(item))
                session.Account.GenericUnlockManager.Unlock((ushort)entry.Id);
        }

        [MessageHandler(GameMessageOpcode.ClientItemUseDecor)]
        public static void HandleItemUseDecor(IWorldSession session, ClientItemUseDecor itemUseDecor)
        {
            IItem item = session.Player.Inventory.GetItem(itemUseDecor.ItemGuid);
            if (item == null)
                throw new InvalidPacketValueException();

            HousingDecorInfoEntry entry = GameTableManager.Instance.HousingDecorInfo.GetEntry(item.Info.Entry.HousingDecorInfoId);
            if (entry == null)
                throw new InvalidPacketValueException();

            if (session.Player.Inventory.ItemUse(item))
                session.Player.ResidenceManager.DecorCreate(entry);
        }

        [MessageHandler(GameMessageOpcode.ClientItemMoveToSupplySatchel)]
        public static void HandleClientItemMoveToSupplySatchel(IWorldSession session, ClientItemMoveToSupplySatchel moveToSupplySatchel)
        {
            IItem item = session.Player.Inventory.GetItem(moveToSupplySatchel.ItemGuid);
            if (item == null)
                throw new InvalidPacketValueException();

            session.Player.Inventory.ItemMoveToSupplySatchel(item, moveToSupplySatchel.Amount);
        }

        [MessageHandler(GameMessageOpcode.ClientItemMoveFromSupplySatchel)]
        public static void HandleClientItemMoveFromSupplySatchel(IWorldSession session, ClientItemMoveFromSupplySatchel request)
        {
            session.Player.SupplySatchelManager.MoveToInventory(request.MaterialId, request.Amount);
        }
    }
}
