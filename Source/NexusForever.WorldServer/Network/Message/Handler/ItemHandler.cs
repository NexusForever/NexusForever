using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Prerequisite;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Game.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class ItemHandler
    {
        [MessageHandler(GameMessageOpcode.ClientItemMove)]
        public static void HandleItemMove(WorldSession session, ClientItemMove itemMove)
        {
            Item item = session.Player.Inventory.GetItem(itemMove.From);
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
        public static void HandleItemSplit(WorldSession session, ClientItemSplit itemSplit)
        {
            session.Player.Inventory.ItemSplit(itemSplit.Guid, itemSplit.Location, itemSplit.Count);
        }

        [MessageHandler(GameMessageOpcode.ClientItemDelete)]
        public static void HandleItemDelete(WorldSession session, ClientItemDelete itemDelete)
        {
            session.Player.Inventory.ItemDelete(itemDelete.From);
        }

        [MessageHandler(GameMessageOpcode.ClientItemUse)]
        public static void HandleItemUse(WorldSession session, ClientItemUse itemUse)
        {
            Item item = session.Player.Inventory.GetItem(itemUse.Location);
            if (item == null)
                throw new InvalidPacketValueException();

            ItemSpecialEntry itemSpecial = GameTableManager.Instance.ItemSpecial.GetEntry(item.Info.Entry.ItemSpecialId00);
            if (itemSpecial == null)
                throw new InvalidPacketValueException();

            if (itemSpecial.Spell4IdOnActivate > 0u)
            {
                if (itemSpecial.PrerequisiteIdGeneric00 > 0 && !PrerequisiteManager.Instance.Meets(session.Player, itemSpecial.PrerequisiteIdGeneric00))
                {
                    session.Player.SendGenericError(Game.Static.GenericError.UnlockItemFailed); // TODO: Confirm right error message.
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

        [MessageHandler(GameMessageOpcode.ClientItemGenericUnlock)]
        public static void HandleItemGenericUnlock(WorldSession session, ClientItemGenericUnlock itemGenericUnlock)
        {
            Item item = session.Player.Inventory.GetItem(itemGenericUnlock.Location);
            if (item == null)
                throw new InvalidPacketValueException();

            GenericUnlockEntryEntry entry = GameTableManager.Instance.GenericUnlockEntry.GetEntry(item.Info.Entry.GenericUnlockSetId);
            if (entry == null)
                throw new InvalidPacketValueException();

            // TODO: should some client error be shown for this?
            if (!session.GenericUnlockManager.IsUnlocked((GenericUnlockType)entry.GenericUnlockTypeEnum, entry.UnlockObject))
                return;

            if (session.Player.Inventory.ItemUse(item))
                session.GenericUnlockManager.Unlock((ushort)entry.Id);
        }

        [MessageHandler(GameMessageOpcode.ClientItemUseDecor)]
        public static void HandleItemUseDecor(WorldSession session, ClientItemUseDecor itemUseDecor)
        {
            Item item = session.Player.Inventory.GetItem(itemUseDecor.ItemGuid);
            if (item == null)
                throw new InvalidPacketValueException();

            HousingDecorInfoEntry entry = GameTableManager.Instance.HousingDecorInfo.GetEntry(item.Info.Entry.HousingDecorInfoId);
            if (entry == null)
                throw new InvalidPacketValueException();

            if (session.Player.Inventory.ItemUse(item))
                session.Player.ResidenceManager.DecorCreate(entry);
        }

        [MessageHandler(GameMessageOpcode.ClientItemMoveToSupplySatchel)]
        public static void HandleClientItemMoveToSupplySatchel(WorldSession session, ClientItemMoveToSupplySatchel moveToSupplySatchel)
        {
            Item item = session.Player.Inventory.GetItem(moveToSupplySatchel.ItemGuid);
            if (item == null)
                throw new InvalidPacketValueException();

            session.Player.Inventory.ItemMoveToSupplySatchel(item, moveToSupplySatchel.Amount);
        }

        [MessageHandler(GameMessageOpcode.ClientItemMoveFromSupplySatchel)]
        public static void HandleClientItemMoveFromSupplySatchel(WorldSession session, ClientItemMoveFromSupplySatchel request)
        {
            session.Player.SupplySatchelManager.MoveToInventory(request.MaterialId, request.Amount);
        }

        [MessageHandler(GameMessageOpcode.ClientItemRuneSocketUnlock)]
        public static void HandleClientItemUnlockRuneSocket(WorldSession session, ClientItemRuneSocketUnlock unlockRuneSocket)
        {
            session.Player.Inventory.RuneSlotUnlock(unlockRuneSocket.Guid, unlockRuneSocket.RuneType, unlockRuneSocket.UseServiceTokens);
        }

        [MessageHandler(GameMessageOpcode.ClientItemRuneSocketReroll)]
        public static void HandleClientItemRuneSocketReroll(WorldSession session, ClientItemRuneSocketReroll runeSocketReroll)
        {
            session.Player.Inventory.RuneSlotReroll(runeSocketReroll.Guid, runeSocketReroll.SocketIndex, runeSocketReroll.RuneType);
        }

        [MessageHandler(GameMessageOpcode.ClientItemRuneInsert)]
        public static void HandleClickItemRuneInsert(WorldSession session, ClientItemRuneInsert runeInsert)
        {
            session.Player.Inventory.RuneInsert(runeInsert.Guid, runeInsert.Glyphs.ToArray());
        }

        [MessageHandler(GameMessageOpcode.ClientItemRuneRemove)]
        public static void HandleClientItemRuneRemove(WorldSession session, ClientItemRuneRemove runeRemove)
        {
            session.Player.Inventory.RuneRemove(runeRemove.Guid, runeRemove.SocketIndex, runeRemove.Recover, runeRemove.UseServiceTokens);
        }
    }
}
