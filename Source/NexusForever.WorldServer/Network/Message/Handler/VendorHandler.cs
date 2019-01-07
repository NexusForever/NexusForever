﻿using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Database.World.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using System.Collections.Generic;

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
            VendorInfo vendorInfo = session.Player.SelectedVendorInfo;
            if (vendorInfo == null)
                return;

            EntityVendorItem vendorItem = vendorInfo.GetItemAtIndex(vendorPurchase.VendorIndex);
            if (vendorItem == null)
                return;

            Item2Entry itemEntry = GameTableManager.Item.GetEntry(vendorItem.ItemId);
            float costMultiplier = vendorInfo.BuyPriceMultiplier * vendorPurchase.VendorItemQty;

            // do all sanity checks before modifying currency
            var currencyChanges = new List<(byte CurrencyTypeId, ulong CurrencyAmount)>();
            for (int i = 0; i < itemEntry.CurrencyTypeId.Length; i++)
            {
                Currency currency = session.Player.CurrencyManager.GetCurrency(itemEntry.CurrencyTypeId[i]);
                if (currency == null)
                    continue;

                ulong currencyAmount = (ulong)(itemEntry.CurrencyAmount[i] * costMultiplier);
                if (currency.Amount < currencyAmount)
                    return;

                currencyChanges.Add((currency.Id, currencyAmount));
            }
            
            foreach ((byte currencyTypeId, ulong currencyAmount) in currencyChanges)
                session.Player.CurrencyManager.CurrencySubtractAmount(currencyTypeId, currencyAmount);

            session.Player.Inventory.ItemCreate(itemEntry.Id, vendorPurchase.VendorItemQty * itemEntry.BuyFromVendorStackCount);
        }


        [MessageHandler(GameMessageOpcode.ClientSellItemToVendor)]
        public static void HandleVendorSell(WorldSession session, ClientVendorSell vendorSell)
        {
            VendorInfo vendorInfo = session.Player.SelectedVendorInfo;
            if (vendorInfo == null)
                return;

            Item2Entry itemEntry = session.Player.Inventory.GetItemFromLocation(vendorSell.ItemLocation).Entry;
            if (itemEntry == null)
                return;

            float costMultiplier = vendorInfo.SellPriceMultiplier * vendorSell.Quantity;

            // do all sanity checks before modifying currency
            var currencyChange = new List<(byte CurrencyTypeId, ulong CurrencyAmount)>();
            for (int i = 0; i < itemEntry.CurrencyTypeIdSellToVendor.Length; i++)
            {
                Currency currency = session.Player.CurrencyManager.GetCurrency(itemEntry.CurrencyTypeIdSellToVendor[i]);
                if (currency == null)
                    continue;

                ulong currencyAmount = (ulong)(itemEntry.CurrencyAmountSellToVendor[i] * costMultiplier);
                currencyChange.Add((currency.Id, currencyAmount));
            }

            // TODO Insert calculation for cost here

            foreach ((byte currencyTypeId, ulong currencyAmount) in currencyChange)
                session.Player.CurrencyManager.CurrencyAddAmount(currencyTypeId, currencyAmount);

            // TODO Figure out why this is showing "You deleted [item]"
            Item soldItem = session.Player.Inventory.ItemDelete(vendorSell.ItemLocation);
            BuybackManager.AddItem(session.Player, soldItem, vendorSell.Quantity, currencyChange);
        }


        [MessageHandler(GameMessageOpcode.ClientBuybackItemFromVendor)]
        public static void HandleBuybackItemFromVendor(WorldSession session, ClientBuybackItemFromVendor buybackItemFromVendor)
        {
            BuybackItem buybackItem = BuybackManager.GetItem(session.Player, buybackItemFromVendor.UniqueId);
            if (buybackItem == null)
                return;

            //TODO Ensure player has room in inventory

            // do all sanity checks before modifying currency
            foreach ((byte currencyTypeId, ulong currencyAmount) in buybackItem.CurrencyChange)
            {
                Currency currency = session.Player.CurrencyManager.GetCurrency(currencyTypeId);
                if (currency != null && currency.Amount < currencyAmount)
                    return;
            }

            foreach ((byte currencyTypeId, ulong currencyAmount) in buybackItem.CurrencyChange)
                session.Player.CurrencyManager.CurrencySubtractAmount(currencyTypeId, currencyAmount);

            session.Player.Inventory.AddItem(buybackItem.Item, InventoryLocation.Inventory);
            BuybackManager.RemoveItem(session.Player, buybackItem);
        }
    }
}
