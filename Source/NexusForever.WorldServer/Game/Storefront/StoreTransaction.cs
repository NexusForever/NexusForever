using NexusForever.Database.Auth.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Account.Static;
using NexusForever.WorldServer.Network.Message.Model;
using System;

namespace NexusForever.WorldServer.Game.Storefront
{
    public class StoreTransaction : IBuildable<ServerStorePurchaseHistory.Purchase>
    {
        public ulong PurchaseId { get; }
        public string Name { get; }
        public AccountCurrencyType CurrencyType { get; }
        public float Cost { get; }
        public DateTime PurchaseDateTime { get; }

        /// <summary>
        /// Create a new <see cref="StoreTransaction"/> for a new purchase.
        /// </summary>
        public StoreTransaction(AccountStoreTransactionModel model)
        {
            PurchaseId       = model.TransactionId;
            Name             = model.Name;
            CurrencyType     = (AccountCurrencyType)model.CurrencyType;
            Cost             = model.Cost;
            PurchaseDateTime = model.TransactionDateTime;
        }

        /// <summary>
        /// Returns a <see cref="ServerStorePurchaseHistory.Purchases"/> representing this <see cref="StoreTransaction"/>.
        /// </summary>
        public ServerStorePurchaseHistory.Purchase Build()
        {
            return new ServerStorePurchaseHistory.Purchase
            {
                PurchaseId = PurchaseId,
                Name = Name,
                CurrencyId = CurrencyType,
                Cost = Cost,
                TransactionDateTime = (ulong)PurchaseDateTime.ToFileTimeUtc()
            };
        }
    }
}
