using NexusForever.Database.Auth.Model;
using NexusForever.Shared.Database;
using NexusForever.Shared.Game.Events;
using NexusForever.WorldServer.Game.Account.Static;
using NexusForever.WorldServer.Game.Storefront.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Game.Storefront
{
    public class PurchaseManager
    {
        private WorldSession session { get; }

        private List<StoreTransaction> transactions = new();

        public bool TransactionsAllowed => transactionsAllowed;

        private bool transactionsAllowed = false;

        /// <summary>
        /// Create a new <see cref="PurchaseManager"/> for this <see cref="WorldSession"/> with a given <see cref="AccountModel"/>.
        /// </summary>
        public PurchaseManager(WorldSession session, AccountModel model)
        {
            this.session = session;

            foreach (AccountStoreTransactionModel transactionModel in model.AccountStoreTransaction)
                transactions.Add(new StoreTransaction(transactionModel));

            transactionsAllowed = true;
        }

        /// <summary>
        /// Complete purchasing an <see cref="OfferItem"/> for the associated account.
        /// </summary>
        public void PurchaseOffer(OfferItem offerItem, AccountCurrencyType accountCurrencyType)
        {
            float cost = offerItem.GetPriceDataForCurrency(accountCurrencyType).GetCurrencyValue();

            // Check for any purchase problems
            StoreError storeError = CheckPurchase(offerItem, accountCurrencyType, cost);
            if (storeError != StoreError.Success)
            {
                session.EnqueueMessageEncrypted(new ServerStoreError
                {
                    ErrorCode = storeError
                });
                return;
            }

            transactionsAllowed = false;

            CreateTransaction(offerItem, accountCurrencyType, cost, () =>
            {
                session.EnqueueMessageEncrypted(new ServerStorePurchaseResult
                {
                    Success = true,
                    ErrorCode = StoreError.Success
                });
                foreach (OfferItemData itemData in offerItem)
                {
                    ulong id = session.AccountInventory.ItemCreate(itemData.Entry);
                    session.AccountInventory.BindItem(id);
                }
                session.AccountCurrencyManager.CurrencySubtractAmount(accountCurrencyType, (ulong)cost);
                transactionsAllowed = true;
            });
        }

        private StoreError CheckPurchase(OfferItem offerItem, AccountCurrencyType accountCurrencyType, float cost)
        {
            if (!TransactionsAllowed)
                return StoreError.PurchasePending;

            if (!session.AccountCurrencyManager.CanAfford(accountCurrencyType, (ulong)cost))
                return StoreError.InvalidPrice;

            // TODO: Check PreReqs to make sure this is purchasable

            return StoreError.Success;
        }

        private void CreateTransaction(OfferItem offerItem, AccountCurrencyType accountCurrencyType, float cost, Action callback)
        {
            AccountStoreTransactionModel transaction = new AccountStoreTransactionModel
            {
                Id = session.Account.Id,
                Name = offerItem.Name,
                CurrencyType = (ushort)accountCurrencyType,
                Cost = cost,
                TransactionDateTime = DateTime.UtcNow
                // TODO: May want to add things like Currency prior to purchase, maybe update this event once item has been delivered to Account Inventory, etc.
            };

            // Create the Transaction Event in DB immediately.
            session.Events.EnqueueEvent(new TaskGenericEvent<AccountStoreTransactionModel>(DatabaseManager.Instance.AuthDatabase.CreateStoreTransaction(session.Account, transaction),
                completedTransaction =>
            {
                transactions.Add(new StoreTransaction(completedTransaction));
                callback.Invoke();
            }));
        }

        /// <summary>
        /// Sends the <see cref="ServerStorePurchaseHistory"/> packet for the <see cref="WorldSession"/> associated with this <see cref="PurchaseManager"/>.
        /// </summary>
        public void SendPurchaseHistory()
        {
            List<ServerStorePurchaseHistory.Purchase> purchases = transactions.Select(x => x.Build()).ToList();
            session.EnqueueMessageEncrypted(new ServerStorePurchaseHistory
            {
             Purchases = purchases   
            });
        }
    }
}
