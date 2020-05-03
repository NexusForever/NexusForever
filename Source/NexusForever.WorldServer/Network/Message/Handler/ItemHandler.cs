using System;
using System.Threading.Tasks;
using NexusForever.Shared.Game.Events;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.Loot;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Game.Prerequisite;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class ItemHandler
    {
        [MessageHandler(GameMessageOpcode.ClientItemMove)]
        public static void HandleItemMove(WorldSession session, ClientItemMove itemMove)
        {
            session.Player.Inventory.ItemMove(itemMove.From, itemMove.To);
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

            ItemSpecialEntry itemSpecial = GameTableManager.Instance.ItemSpecial.GetEntry(item.Entry.ItemSpecialId00);
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

            GenericUnlockEntryEntry entry = GameTableManager.Instance.GenericUnlockEntry.GetEntry(item.Entry.GenericUnlockSetId);
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

            HousingDecorInfoEntry entry = GameTableManager.Instance.HousingDecorInfo.GetEntry(item.Entry.HousingDecorInfoId);
            if (entry == null)
                throw new InvalidPacketValueException();

            Task<Residence> task = ResidenceManager.Instance.GetResidence(session.Player.Name);
            session.EnqueueEvent(new TaskGenericEvent<Residence>(task,
                residence =>
            {
                if (residence == null)
                    residence = ResidenceManager.Instance.CreateResidence(session.Player);

                if (session.Player.Inventory.ItemUse(item))
                {
                    if (session.Player.Map is ResidenceMap residenceMap)
                        residenceMap.DecorCreate(entry, 1);
                    else
                        residence.DecorCreate(entry);
                }
            }));
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

        [MessageHandler(GameMessageOpcode.ClientItemUseLootBag)]
        public static void HandleClientItemUseLootBag(WorldSession session, ClientItemUseLootBag useLootBag)
        {
            Item item = session.Player.Inventory.GetItem(useLootBag.ItemLocation);
            if (item == null)
                throw new ArgumentException($"Item missing at Inventory Location {useLootBag.ItemLocation.Location} and Index {useLootBag.ItemLocation.BagIndex}.");

            if (useLootBag.Guid != item.Guid)
                throw new InvalidOperationException($"Guid {useLootBag.Guid} received does not match the Item found at Inventory Location {useLootBag.ItemLocation.Location} and Index {useLootBag.ItemLocation.BagIndex}.");

            if (item.Entry.Item2CategoryId != 138)
                throw new NotImplementedException();

            if (session.Player.Inventory.ItemUse(item))
                GlobalLootManager.Instance.DropLoot(session, item);
        }
    }
}
