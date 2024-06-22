using System;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Account;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Vendor
{
    public class ClientVendorPurchaseHandler : IMessageHandler<IWorldSession, ClientVendorPurchase>
    {
        #region Dependency Injection

        private readonly IItemManager itemManager;

        public ClientVendorPurchaseHandler(
            IItemManager itemManager)
        {
            this.itemManager = itemManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientVendorPurchase vendorPurchase)
        {
            IVendorInfo vendorInfo = session.Player.SelectedVendorInfo;
            if (vendorInfo == null)
                return;

            EntityVendorItemModel vendorItem = vendorInfo.GetItemAtIndex(vendorPurchase.VendorIndex);
            if (vendorItem == null)
                return;

            IItemInfo info = itemManager.GetItemInfo(vendorItem.ItemId);
            if (info == null)
                return;

            var vendorItemPurchaseCost = new VendorItemPurchaseCost();

            if (vendorItem.ExtraCost1Type == ItemExtraCostType.None
                && vendorItem.ExtraCost2Type == ItemExtraCostType.None)
            {
                for (byte i = 0; i < info.Entry.CurrencyTypeId.Length; i++)
                {
                    CurrencyType currencyType = info.GetVendorBuyCurrency(i);
                    if (currencyType == CurrencyType.None)
                        continue;

                    ulong cost = info.GetVendorBuyAmount(i) * vendorPurchase.VendorItemQty;
                    if (currencyType == CurrencyType.Credits)
                        cost *= (ulong)Math.Ceiling(vendorInfo.BuyPriceMultiplier);

                    vendorItemPurchaseCost.AddCurrencyCost(currencyType, cost);
                }
            }
            else
            {
                void AddExtraCost(ItemExtraCostType type, uint id, uint cost)
                {
                    switch (type)
                    {
                        case ItemExtraCostType.Item:
                            vendorItemPurchaseCost.AddItemCost(id, cost);
                            break;
                        case ItemExtraCostType.Currency:
                            vendorItemPurchaseCost.AddCurrencyCost((CurrencyType)id, cost);
                            break;
                        case ItemExtraCostType.AccountCurrency:
                            vendorItemPurchaseCost.AddAccountCurrencyCost((AccountCurrencyType)id, cost);
                            break;
                    }
                }

                AddExtraCost(vendorItem.ExtraCost1Type, vendorItem.ExtraCost1ItemOrCurrencyId, vendorItem.ExtraCost1Quantity);
                AddExtraCost(vendorItem.ExtraCost2Type, vendorItem.ExtraCost2ItemOrCurrencyId, vendorItem.ExtraCost2Quantity);
            }

            if (!vendorItemPurchaseCost.CanAfford(session.Player))
                return;

            vendorItemPurchaseCost.Charge(session.Player);
            session.Player.Inventory.ItemCreate(InventoryLocation.Inventory, info.Id, vendorPurchase.VendorItemQty * info.Entry.BuyFromVendorStackCount);
        }
    }
}
