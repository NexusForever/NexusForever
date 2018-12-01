﻿using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Database.World.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System;

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
            var serverVendor = new ServerVendor
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
                serverVendor.Categories.Add(new ServerVendor.Category
                {
                    Index = category.Index,
                    LocalisedTextId = category.LocalisedTextId
                });
            }
            foreach (EntityVendorItem item in vendorEntity.VendorInfo.Items)
            {
                serverVendor.Items.Add(new ServerVendor.Item
                {
                    Index = item.Index,
                    ItemId = item.ItemId,
                    CategoryIndex = item.CategoryIndex,
                    Unknown6 = 0,
                    UnknownB = new[]
                    {
                        new ServerVendor.Item.UnknownItemStructure(),
                        new ServerVendor.Item.UnknownItemStructure()
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

            if (currency0 != null && currency0.Count < itemEntry.CurrencyAmount0 * costMultiplier)
                return;

            if (currency1 != null && currency1.Count < itemEntry.CurrencyAmount1 * costMultiplier)
                return;

            if (currency0 != null)
                session.Player.CurrencyManager.CurrencySubtractCount(currency0.Entry, (uint)(itemEntry.CurrencyAmount0 * costMultiplier));

            if (currency1 != null)
                session.Player.CurrencyManager.CurrencySubtractCount(currency1.Entry, (uint)(itemEntry.CurrencyAmount1 * costMultiplier));

            var item = new Game.Entity.Item(session.Player.CharacterId, itemEntry, Math.Min(1, itemEntry.MaxStackCount));
            session.Player.Inventory.ItemCreate(itemEntry.Id, vendorPurchase.VendorItemQty * itemEntry.BuyFromVendorStackCount);
        }


        [MessageHandler(GameMessageOpcode.ClientVendorSell)]
        public static void HandleVendorSell(WorldSession session, ClientVendorSell vendorSell)
        {
            VendorInfo VendorInfo = session.Player.SelectedVendorInfo;

            if (VendorInfo == null)
                return;

            Item2Entry itemEntry = session.Player.Inventory.GetItemFromLocation(vendorSell.ItemLocation).Entry;

            if (itemEntry == null)
                return;

            session.Player.Inventory.ItemDelete(vendorSell.ItemLocation);

            float sellMultiplier = VendorInfo.SellPriceMultiplier * vendorSell.Count;
            Currency currency0 = session.Player.CurrencyManager.GetCurrency(itemEntry.CurrencyTypeId0);
            Currency currency1 = session.Player.CurrencyManager.GetCurrency(itemEntry.CurrencyTypeId1);

            if (currency0 != null)
                session.Player.CurrencyManager.CurrencyAddCount((byte)itemEntry.CurrencyTypeId0, (uint)(itemEntry.CurrencyAmount0SellToVendor * sellMultiplier));

            if (currency1 != null)
                session.Player.CurrencyManager.CurrencyAddCount((byte)itemEntry.CurrencyTypeId1, (uint)(itemEntry.CurrencyAmount1SellToVendor * sellMultiplier));
        }
    }
}