using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Account;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Game.Entity
{
    /// <summary>
    /// Represents the purchase cost of an item from a vendor.
    /// </summary>
    public class VendorItemPurchaseCost : IVendorItemPurchaseCost
    {
        private readonly Dictionary<CurrencyType, ulong> currencyCost = new();
        private readonly Dictionary<AccountCurrencyType, ulong> accountCurrencyCost = new();
        private readonly Dictionary<uint, uint> itemCost = new();

        /// <summary>
        /// Add a <see cref="CurrencyType"/> cost to the purchase cost.
        /// </summary>
        public void AddCurrencyCost(CurrencyType currencyType, ulong cost)
        {
            if (currencyCost.ContainsKey(currencyType))
                currencyCost[currencyType] += cost;
            else
                currencyCost.Add(currencyType, cost);
        }

        /// <summary>
        /// Add a <see cref="AccountCurrencyType"/> cost to the purchase cost.
        /// </summary>
        public void AddAccountCurrencyCost(AccountCurrencyType currencyType, ulong cost)
        {
            if (accountCurrencyCost.ContainsKey(currencyType))
                accountCurrencyCost[currencyType] += cost;
            else
                accountCurrencyCost.Add(currencyType, cost);
        }

        /// <summary>
        /// Add an item cost to the purchase cost.
        /// </summary>
        public void AddItemCost(uint itemId, uint cost)
        {
            if (itemCost.ContainsKey(itemId))
                itemCost[itemId] += cost;
            else
                itemCost.Add(itemId, cost);
        }

        /// <summary>
        /// Returns if the <see cref="IPlayer"/> can afford the purchase cost.
        /// </summary>
        public bool CanAfford(IPlayer player)
        {
            foreach ((CurrencyType currencyTypeId, ulong currencyAmount) in currencyCost)
                if (!player.CurrencyManager.CanAfford(currencyTypeId, currencyAmount))
                    return false;

            foreach ((AccountCurrencyType currencyTypeId, ulong currencyAmount) in accountCurrencyCost)
                if (!player.Account.CurrencyManager.CanAfford(currencyTypeId, currencyAmount))
                    return false;

            foreach ((uint itemId, uint itemCount) in itemCost)
                if (!player.Inventory.HasItemCount(itemId, itemCount))
                    return false;

            return true;
        }

        /// <summary>
        /// Charge the purchase cost to the <see cref="IPlayer"/>.
        /// </summary>
        public void Charge(IPlayer player)
        {
            foreach ((CurrencyType currencyTypeId, ulong currencyCost) in currencyCost)
                player.CurrencyManager.CurrencySubtractAmount(currencyTypeId, currencyCost);

            foreach ((AccountCurrencyType currencyTypeId, ulong currencyCost) in accountCurrencyCost)
                player.Account.CurrencyManager.CurrencySubtractAmount(currencyTypeId, currencyCost);

            foreach ((uint itemId, uint itemAmount) in itemCost)
                player.Inventory.ItemDelete(itemId, itemAmount, ItemUpdateReason.Vendor);
        }
    }
}
