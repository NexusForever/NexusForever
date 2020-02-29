using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Account.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Account
{
    public class AccountCurrencyManager
    {
        private readonly WorldSession session;
        private readonly Dictionary<AccountCurrencyType, AccountCurrency> currencies = new Dictionary<AccountCurrencyType, AccountCurrency>();

        public AccountCurrencyManager(WorldSession session, AccountModel model)
        {
            this.session = session;

            foreach (AccountCurrencyModel currencyModel in model.AccountCurrency)
            {
                // Disabled Character Token for now due to causing server errors if the player tries to use it. TODO: Fix level 50 creation
                if ((AccountCurrencyType)currencyModel.CurrencyId == AccountCurrencyType.MaxLevelToken)
                    continue;

                currencies.Add((AccountCurrencyType)currencyModel.CurrencyId, new AccountCurrency(currencyModel));
            }
        }

        public void Save(AuthContext context)
        {
            foreach (AccountCurrency accountCurrency in currencies.Values)
                accountCurrency.Save(context);
        }

        /// <summary>
        /// Create a new <see cref="AccountCurrency"/>.
        /// </summary>
        private AccountCurrency CreateAccountCurrency(AccountCurrencyType currencyType, ulong amount = 0)
        {
            AccountCurrencyTypeEntry currencyEntry = GameTableManager.Instance.AccountCurrencyType.GetEntry((ulong)currencyType);
            if (currencyEntry == null)
                throw new ArgumentNullException($"AccountCurrencyTypeEntry not found for currencyId {currencyType}");

            if (currencies.TryAdd(currencyType, new AccountCurrency(session.Account.Id, currencyType, amount)))
                return currencies[currencyType];
            else
                return null;
        }

        /// <summary>
        /// Returns whether the Account has enough of the currency to afford the amount
        /// </summary>
        public bool CanAfford(AccountCurrencyType currencyType, ulong amount)
        {
            if (!currencies.TryGetValue(currencyType, out AccountCurrency accountCurrency))
                return false;

            return accountCurrency.CanAfford(amount);
        }

        /// <summary>
        /// Add a supplied amount to an <see cref="AccountCurrency"/>.
        /// </summary>
        public void CurrencyAddAmount(AccountCurrencyType currencyType, ulong amount, ulong reason = 0)
        {
            if (!currencies.TryGetValue(currencyType, out AccountCurrency accountCurrency))
            {
                accountCurrency = CreateAccountCurrency(currencyType, 0);
            }

            if (accountCurrency == null)
                throw new ArgumentException($"Account Currency entry not found for currencyId {currencyType}.");

            if (accountCurrency.AddAmount(amount))
                SendAccountCurrencyUpdate(accountCurrency, reason);
        }

        /// <summary>
        /// Subtract a supplied amount to an <see cref="AccountCurrency"/>.
        /// </summary>
        public void CurrencySubtractAmount(AccountCurrencyType currencyType, ulong amount, ulong reason = 0)
        {
            if (!currencies.TryGetValue(currencyType, out AccountCurrency accountCurrency))
            {
                accountCurrency = CreateAccountCurrency(currencyType, 0);
            }

            if (accountCurrency == null)
                throw new ArgumentException($"Account Currency entry not found for currencyId {currencyType}.");

            if (!accountCurrency.CanAfford(amount))
                throw new ArgumentException($"Trying to remove more currency {accountCurrency.CurrencyId} than the player has!");

            // TODO: Ensure that we're not at cap - is there a cap?
            if(accountCurrency.SubtractAmount(amount))
                SendAccountCurrencyUpdate(accountCurrency, reason);
        }

        /// <summary>
        /// Sends information about all the player's <see cref="AccountCurrency"/> during Character Select
        /// </summary>
        public void SendCharacterListPacket()
        {
            session.EnqueueMessageEncrypted(new ServerAccountCurrencySet
            {
                AccountCurrencies = currencies.Values.Select(c => c.Build()).ToList()
            });
        }

        /// <summary>
        /// Sends information about all the player's <see cref="AccountCurrency"/> when entering world
        /// </summary>
        public void SendInitialPackets()
        {
            foreach (AccountCurrency accountCurrency in currencies.Values)
                SendAccountCurrencyUpdate(accountCurrency);
        }

        /// <summary>
        /// Sends information about a player's <see cref="AccountCurrency"/>
        /// </summary>
        private void SendAccountCurrencyUpdate(AccountCurrency accountCurrency, ulong reason = 0)
        {
            session.EnqueueMessageEncrypted(new ServerAccountCurrencyGrant
            {
                AccountCurrency = accountCurrency.Build(),
                Unknown0 = reason
            });
        }
    }
}
