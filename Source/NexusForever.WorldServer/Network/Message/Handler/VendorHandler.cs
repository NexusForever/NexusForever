using System;
using System.Collections.Generic;
using NexusForever.Database.World.Model;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class VendorHandler
    {
        public static void HandleClientVendor(IWorldSession session, ClientEntityInteract vendor)
        {
            var vendorEntity = session.Player.Map.GetEntity<INonPlayer>(vendor.Guid);
            if (vendorEntity == null)
                throw new InvalidOperationException();

            if (vendorEntity.VendorInfo == null)
                throw new InvalidOperationException();

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

            foreach (EntityVendorCategoryModel category in vendorEntity.VendorInfo.Categories)
            {
                serverVendor.Categories.Add(new ServerVendorItemsUpdated.Category
                {
                    Index = category.Index,
                    LocalisedTextId = category.LocalisedTextId
                });
            }
            foreach (EntityVendorItemModel item in vendorEntity.VendorInfo.Items)
            {
                serverVendor.Items.Add(new ServerVendorItemsUpdated.Item
                {
                    Index = item.Index,
                    ItemId = item.ItemId,
                    CategoryIndex = item.CategoryIndex,
                    Unknown6 = 0,
                    ExtraCost1 = new ServerVendorItemsUpdated.Item.ItemExtraCost()
                        {
                            ExtraCostType = (ServerVendorItemsUpdated.Item.ItemExtraCost.ItemExtraCostType)item.ExtraCost1Type,
                            Quantity = item.ExtraCost1Quantity,
                            ItemOrCurrencyId = item.ExtraCost1ItemOrCurrencyId
                        },
                    ExtraCost2 = new ServerVendorItemsUpdated.Item.ItemExtraCost()
                        {
                            ExtraCostType = (ServerVendorItemsUpdated.Item.ItemExtraCost.ItemExtraCostType)item.ExtraCost2Type,
                            Quantity = item.ExtraCost2Quantity,
                            ItemOrCurrencyId = item.ExtraCost2ItemOrCurrencyId
                        }
                });
            }
            session.EnqueueMessageEncrypted(serverVendor);
        }

        [MessageHandler(GameMessageOpcode.ClientVendorPurchase)]
        public static void HandleVendorPurchase(IWorldSession session, ClientVendorPurchase vendorPurchase)
        {
            IVendorInfo vendorInfo = session.Player.SelectedVendorInfo;
            if (vendorInfo == null)
                return;

            EntityVendorItemModel vendorItem = vendorInfo.GetItemAtIndex(vendorPurchase.VendorIndex);
            if (vendorItem == null)
                return;

            Item2Entry itemEntry = GameTableManager.Instance.Item.GetEntry(vendorItem.ItemId);
            float costMultiplier = vendorInfo.BuyPriceMultiplier * vendorPurchase.VendorItemQty;

            IItemInfo info = ItemManager.Instance.GetItemInfo(itemEntry.Id);
            if (info == null)
                return;

            var useExtraCosts = (vendorItem.ExtraCost1Type + vendorItem.ExtraCost2Type) > 0;
            // do all sanity checks before modifying currency
            var currencyChanges = new List<(CurrencyType CurrencyTypeId, ulong CurrencyAmount)>();

            if (!useExtraCosts)
            {
                for (int i = 0; i < itemEntry.CurrencyTypeId.Length; i++)
                {
                    CurrencyType currencyId = (CurrencyType)itemEntry.CurrencyTypeId[i];
                    if (currencyId == CurrencyType.None)
                        continue;

                    ulong currencyAmount = (ulong)(itemEntry.CurrencyAmount[i] * costMultiplier);
                    if (!session.Player.CurrencyManager.CanAfford(currencyId, currencyAmount))
                        return;

                    currencyChanges.Add((currencyId, currencyAmount));
                }
                
                // TODO Calculate values appropriately.
                // clientPurchaseMod was confirmed by in-game vendor values. Need to find GameTable it's associated with.
                ulong amount = (ulong)Math.Ceiling(info.GetVendorBuyAmount(0) * costMultiplier);
                currencyChanges.Add((CurrencyType.Credits, amount));
            }
            else // useExtraCosts
            {
                if (vendorItem.ExtraCost1Type == (byte)ServerVendorItemsUpdated.Item.ItemExtraCost.ItemExtraCostType.Currency)
                {
                    if (!session.Player.CurrencyManager.CanAfford((CurrencyType)vendorItem.ExtraCost1ItemOrCurrencyId,
                            vendorItem.ExtraCost1Quantity))
                        return;
                    currencyChanges.Add(((CurrencyType)vendorItem.ExtraCost1ItemOrCurrencyId, vendorItem.ExtraCost1Quantity));
                }

                if (vendorItem.ExtraCost2Type == (byte)ServerVendorItemsUpdated.Item.ItemExtraCost.ItemExtraCostType.Currency)
                {
                    if (!session.Player.CurrencyManager.CanAfford((CurrencyType)vendorItem.ExtraCost2ItemOrCurrencyId,
                            vendorItem.ExtraCost2Quantity))
                        return;
                    currencyChanges.Add(((CurrencyType)vendorItem.ExtraCost2ItemOrCurrencyId, vendorItem.ExtraCost2Quantity));
                }

                if (vendorItem.ExtraCost1Type ==
                    (byte)ServerVendorItemsUpdated.Item.ItemExtraCost.ItemExtraCostType.Item)
                {
                    // TODO: Lacks a sanity check
                    session.Player.Inventory.ItemDelete(vendorItem.ExtraCost1ItemOrCurrencyId,
                        vendorItem.ExtraCost1Quantity, ItemUpdateReason.Vendor);
                }
                
                if (vendorItem.ExtraCost2Type ==
                    (byte)ServerVendorItemsUpdated.Item.ItemExtraCost.ItemExtraCostType.Item)
                {
                    // TODO: Lacks a sanity check
                    session.Player.Inventory.ItemDelete(vendorItem.ExtraCost2ItemOrCurrencyId,
                        vendorItem.ExtraCost2Quantity, ItemUpdateReason.Vendor);
                }
            }

            foreach ((CurrencyType currencyTypeId, ulong currencyAmount) in currencyChanges)
                session.Player.CurrencyManager.CurrencySubtractAmount(currencyTypeId, currencyAmount);

            session.Player.Inventory.ItemCreate(InventoryLocation.Inventory, itemEntry.Id, vendorPurchase.VendorItemQty * itemEntry.BuyFromVendorStackCount);
        }

        [MessageHandler(GameMessageOpcode.ClientSellItemToVendor)]
        public static void HandleVendorSell(IWorldSession session, ClientVendorSell vendorSell)
        {
            IVendorInfo vendorInfo = session.Player.SelectedVendorInfo;
            if (vendorInfo == null)
                return;

            IItem item = session.Player.Inventory.GetItem(vendorSell.ItemLocation);
            if (item == null)
                return;

            IItemInfo info = item.Info;
            if (info == null)
                return;

            float costMultiplier = vendorInfo.SellPriceMultiplier * vendorSell.Quantity;

            // do all sanity checks before modifying currency
            var currencyChange = new List<(CurrencyType CurrencyTypeId, ulong CurrencyAmount)>();
            for (int i = 0; i < info.Entry.CurrencyTypeIdSellToVendor.Length; i++)
            {
                CurrencyType currencyId = (CurrencyType)info.Entry.CurrencyTypeIdSellToVendor[i];
                if (currencyId == CurrencyType.None)
                    continue;

                ulong currencyAmount = (ulong)(info.Entry.CurrencyAmountSellToVendor[i] * costMultiplier);
                currencyChange.Add((currencyId, currencyAmount));
            }

            // TODO Insert calculation for cost here
            currencyChange.Add((CurrencyType.Credits, (ulong)(item.GetVendorSellAmount(0) * costMultiplier)));

            foreach ((CurrencyType currencyTypeId, ulong currencyAmount) in currencyChange)
                session.Player.CurrencyManager.CurrencyAddAmount(currencyTypeId, currencyAmount);

            // TODO Figure out why this is showing "You deleted [item]"
            IItem soldItem = session.Player.Inventory.ItemDelete(vendorSell.ItemLocation, ItemUpdateReason.Vendor);
            BuybackManager.Instance.AddItem(session.Player, soldItem, vendorSell.Quantity, currencyChange);
        }

        [MessageHandler(GameMessageOpcode.ClientBuybackItemFromVendor)]
        public static void HandleBuybackItemFromVendor(IWorldSession session, ClientBuybackItemFromVendor buybackItemFromVendor)
        {
            IBuybackItem buybackItem = BuybackManager.Instance.GetItem(session.Player, buybackItemFromVendor.UniqueId);
            if (buybackItem == null)
                return;

            //TODO Ensure player has room in inventory
            if (session.Player.Inventory.GetInventorySlotsRemaining(InventoryLocation.Inventory) < 1)
            {
                session.Player.SendGenericError(GenericError.ItemInventoryFull);
                return;
            }

            // do all sanity checks before modifying currency
            foreach ((CurrencyType currencyTypeId, ulong currencyAmount) in buybackItem.CurrencyChange)
                if (!session.Player.CurrencyManager.CanAfford(currencyTypeId, currencyAmount))
                    return;

            foreach ((CurrencyType currencyTypeId, ulong currencyAmount) in buybackItem.CurrencyChange)
                session.Player.CurrencyManager.CurrencySubtractAmount(currencyTypeId, currencyAmount);

            session.Player.Inventory.AddItem(buybackItem.Item, InventoryLocation.Inventory);
            BuybackManager.Instance.RemoveItem(session.Player, buybackItem);
        }
    }
}
