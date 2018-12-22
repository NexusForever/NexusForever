using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Database.World.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
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
            Dictionary<CurrencyTypeEntry, ulong> subtractions = new Dictionary<CurrencyTypeEntry, ulong>();
            for (int i = 0; i < itemEntry.CurrencyTypeId.Length; i++)
            {
                Currency currency = session.Player.CurrencyManager.GetCurrency(itemEntry.CurrencyTypeId[i]);
                if (currency == null)
                    continue;
                ulong calculatedCost = (ulong)(itemEntry.CurrencyAmount[i] * costMultiplier);
                if (currency != null && currency.Amount < calculatedCost)
                    return; // Player does not have enough currency, abort
                subtractions[currency.Entry] = calculatedCost;
            }
            
            foreach (KeyValuePair<CurrencyTypeEntry, ulong> entry in subtractions)
                session.Player.CurrencyManager.CurrencySubtractAmount(entry.Key, entry.Value);

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

            Dictionary<CurrencyTypeEntry, ulong> additions = new Dictionary<CurrencyTypeEntry, ulong>();
            for (int i = 0; i < itemEntry.CurrencyTypeId.Length; i++)
            {
                Currency currency = session.Player.CurrencyManager.GetCurrency(itemEntry.CurrencyTypeId[i]);
                if (currency == null)
                    continue;
                ulong calculatedCost = (ulong)(itemEntry.CurrencyAmount[i] * sellMultiplier);
                additions[currency.Entry] = calculatedCost;
            }

            // TODO Insert calculation for cost here

            // TODO Figure out why this is showing "You deleted [item]"
            Item soldItem = session.Player.Inventory.ItemDelete(vendorSell.ItemLocation);

            foreach (KeyValuePair<CurrencyTypeEntry, ulong> entry in additions)
                session.Player.CurrencyManager.CurrencyAddAmount(entry.Key, entry.Value);

            BuybackItem buybackItem = new BuybackItem
            {
                Item = soldItem,
                CurrencyAdditions = additions,
                BuybackItemId = session.Player.highestBuybackId++,
                Quantity = vendorSell.Quantity
            };

            session.Player.BuybackItems.Add(buybackItem.BuybackItemId, buybackItem);

            session.EnqueueMessageEncrypted(new ServerBuybackItemsUpdated
            {
                BuybackItem = buybackItem
            });
        }


        [MessageHandler(GameMessageOpcode.ClientBuybackItemFromVendor)]
        public static void HandleBuybackItemFromVendor(WorldSession session, ClientBuybackItemFromVendor buybackItemFromVendor)
        {

            if (!session.Player.BuybackItems.TryGetValue(buybackItemFromVendor.BuybackPosition, out BuybackItem buybackItem))
                return; //TODO tell client it failed

            //TODO Ensure player has room in inventory

            foreach (KeyValuePair<CurrencyTypeEntry, ulong> entry in buybackItem.CurrencyAdditions)
            {
                Currency currency = session.Player.CurrencyManager.GetCurrency(entry.Key.Id);
                if (currency != null && currency.Amount < entry.Value)
                    return;  // Player does not have enough currency, abort
            }

            foreach (KeyValuePair<CurrencyTypeEntry, ulong> entry in buybackItem.CurrencyAdditions)
                session.Player.CurrencyManager.CurrencySubtractAmount(entry.Key, entry.Value);

            session.Player.Inventory.AddItem(buybackItem.Item, buybackItem.Item.Location);
            session.EnqueueMessageEncrypted(new ServerBuybackItemRemoved
            {
                BuybackItemId = buybackItem.BuybackItemId
            });
        }
    }
}
