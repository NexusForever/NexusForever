using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Database.World.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Linq;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class VendorHandler
    {

        [MessageHandler(GameMessageOpcode.ClientVendor)]
        public static void HandleClientVendor(WorldSession session, ClientVendor vendor)
        {
            var vendorEntity = session.Player.Map.GetEntity<NonPlayer>(vendor.Guid);
            if (vendorEntity == null)
            {
                return;
            }

            if (vendorEntity.VendorInfo == null)
            {
                return;
            }

            session.Player.SelectedVendorInfo = vendorEntity.VendorInfo;
            var serverVendor = new ServerVendorItemsUpdated
            {
                Guid = vendor.Guid,
                SellPriceMultiplier = vendorEntity.VendorInfo.SellPriceMultiplier,
                BuyPriceMultiplier = vendorEntity.VendorInfo.BuyPriceMultiplier,
                Unknown2 = true,
                Unknown3 = true,
                Unknown4 = false
            };

            foreach (EntityVendorCategory category in vendorEntity.VendorInfo.Categories)
            {
                serverVendor.Categories.Add(new ServerVendorItemsUpdated.Category
                {
                    Index = category.Index,
                    LocalisedTextId = category.LocalisedTextId
                });
            }
            foreach (EntityVendorItem item in vendorEntity.VendorInfo.Items)
            {
                serverVendor.Items.Add(new ServerVendorItemsUpdated.Item
                {
                    Index = item.Index,
                    ItemId = item.ItemId,
                    CategoryIndex = item.CategoryIndex,
                    Unknown6 = 0,
                    UnknownB = new[]
                    {
                        new ServerVendorItemsUpdated.Item.UnknownItemStructure(),
                        new ServerVendorItemsUpdated.Item.UnknownItemStructure()
                    }
                });
            }
            session.EnqueueMessageEncrypted(serverVendor);
        }

        [MessageHandler(GameMessageOpcode.ClientVendorPurchase)]
        public static void HandleVendorPurchase(WorldSession session, ClientVendorPurchase vendorPurchase)
        {
            VendorInfo VendorInfo = session.Player.SelectedVendorInfo;

            if (VendorInfo == null)
                return;

            EntityVendorItem vendorItem = VendorInfo.GetItemAtIndex(vendorPurchase.VendorIndex);
            if (vendorItem == null)
                return;

            Item2Entry itemEntry = GameTableManager.Item.GetEntry(vendorItem.ItemId);
            float costMultiplier = vendorPurchase.VendorItemQty * VendorInfo.BuyPriceMultiplier;
            Currency currency0 = session.Player.CurrencyManager.GetCurrency(itemEntry.CurrencyTypeId0);
            Currency currency1 = session.Player.CurrencyManager.GetCurrency(itemEntry.CurrencyTypeId1);
            ulong calculatedCost0 = (ulong)(itemEntry.CurrencyAmount0 * costMultiplier);
            ulong calculatedCost1 = (ulong)(itemEntry.CurrencyAmount1 * costMultiplier);

            if (currency0 != null && currency0.Amount < itemEntry.CurrencyAmount0 * costMultiplier)
                return;

            if (currency1 != null && currency1.Amount < itemEntry.CurrencyAmount1 * costMultiplier)
                return;

            if (currency0 != null)
                session.Player.CurrencyManager.CurrencySubtractAmount(currency0.Entry, calculatedCost0);

            if (currency1 != null)
                session.Player.CurrencyManager.CurrencySubtractAmount(currency1.Entry, calculatedCost1);

            var item = new Game.Entity.Item(session.Player.CharacterId, itemEntry, Math.Min(1, itemEntry.MaxStackCount));
            session.Player.Inventory.ItemCreate(itemEntry.Id, vendorPurchase.VendorItemQty * itemEntry.BuyFromVendorStackCount);
        }


        [MessageHandler(GameMessageOpcode.ClientSellItemToVendor)]
        public static void HandleVendorSell(WorldSession session, ClientVendorSell vendorSell)
        {
            VendorInfo VendorInfo = session.Player.SelectedVendorInfo;

            if (VendorInfo == null)
                return;

            Item2Entry itemEntry = session.Player.Inventory.GetItemFromLocation(vendorSell.ItemLocation).Entry;

            if (itemEntry == null)
                return;

            float sellMultiplier = VendorInfo.SellPriceMultiplier * vendorSell.Quantity;
            Currency currency0 = session.Player.CurrencyManager.GetCurrency(itemEntry.CurrencyTypeId0);
            Currency currency1 = session.Player.CurrencyManager.GetCurrency(itemEntry.CurrencyTypeId1);
            ulong calculatedCost0 = (uint)(itemEntry.CurrencyAmount0SellToVendor * sellMultiplier);
            ulong calculatedCost1 = (uint)(itemEntry.CurrencyAmount1SellToVendor * sellMultiplier);

            if (currency0 == null)
                currency0 = session.Player.CurrencyManager.GetCurrency(1);
            if (currency1 == null)
                currency1 = session.Player.CurrencyManager.GetCurrency(1);


            // TODO Insert calculation for cost here
            if (calculatedCost0 == 0 && calculatedCost1 == 0)
                calculatedCost0 = 1;

            // TODO Figure out why this is showing "You deleted [item]"
            Item soldItem = session.Player.Inventory.ItemDelete(vendorSell.ItemLocation);
            BuybackItem buybackItem = new BuybackItem
            {
                Item = soldItem,
                CurrencyTypeId0 = (byte)currency0.Entry.Id,
                CurrencyTypeId1 = (byte)currency1.Entry.Id,
                CurrencyAmount0 = calculatedCost0,
                CurrencyAmount1 = calculatedCost1,
                BuybackItemId = session.Player.highestBuybackId++,
                Quantity = vendorSell.Quantity
            };
            session.Player.BuybackItems.Add(buybackItem.BuybackItemId, buybackItem);

            session.EnqueueMessageEncrypted(new ServerBuybackItemsUpdated
            {
                BuybackItem = buybackItem
            });

            if (currency0 != null)
                session.Player.CurrencyManager.CurrencyAddAmount((byte)currency0.Entry.Id, calculatedCost0);

            if (currency1 != null)
                session.Player.CurrencyManager.CurrencyAddAmount((byte)currency1.Entry.Id, calculatedCost1);
        }


        [MessageHandler(GameMessageOpcode.ClientBuybackItemFromVendor)]
        public static void HandleBuybackItemFromVendor(WorldSession session, ClientBuybackItemFromVendor buybackItemFromVendor)
        {

            if (!session.Player.BuybackItems.TryGetValue(buybackItemFromVendor.BuybackPosition, out BuybackItem buybackItem))
                return; //TODO tell client it failed

            //TODO Ensure player has room in inventory

            Currency currency0 = session.Player.CurrencyManager.GetCurrency(buybackItem.CurrencyTypeId0);
            Currency currency1 = session.Player.CurrencyManager.GetCurrency(buybackItem.CurrencyTypeId1);

            if (currency0 != null && currency0.Amount < buybackItem.CurrencyAmount0)
                return;

            if (currency1 != null && currency1.Amount < buybackItem.CurrencyAmount1)
                return;

            if (currency0 != null)
                session.Player.CurrencyManager.CurrencySubtractAmount(currency0.Entry, buybackItem.CurrencyAmount0);

            if (currency1 != null)
                session.Player.CurrencyManager.CurrencySubtractAmount(currency1.Entry, buybackItem.CurrencyAmount1);

            session.Player.Inventory.AddItem(buybackItem.Item, buybackItem.Item.Location);
            session.EnqueueMessageEncrypted(new ServerBuybackItemRemoved
            {
                BuybackItemId = buybackItem.BuybackItemId
            });
        }
    }
}
