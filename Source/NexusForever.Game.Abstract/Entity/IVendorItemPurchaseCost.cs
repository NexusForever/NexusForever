using NexusForever.Game.Static.Account;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    /// <summary>
    /// Represents the purchase cost of an item from a vendor.
    /// </summary>
    public interface IVendorItemPurchaseCost
    {
        /// <summary>
        /// Add a <see cref="CurrencyType"/> cost to the purchase cost.
        /// </summary>
        void AddAccountCurrencyCost(AccountCurrencyType currencyType, ulong cost);

        /// <summary>
        /// Add a <see cref="AccountCurrencyType"/> cost to the purchase cost.
        /// </summary>
        void AddCurrencyCost(CurrencyType currencyId, ulong cost);

        /// <summary>
        /// Add an item cost to the purchase cost.
        /// </summary>
        void AddItemCost(uint itemId, uint cost);

        /// <summary>
        /// Returns if the <see cref="IPlayer"/> can afford the purchase cost.
        /// </summary>
        bool CanAfford(IPlayer player);

        /// <summary>
        /// Charge the purchase cost to the <see cref="IPlayer"/>.
        /// </summary>
        void Charge(IPlayer player);
    }
}