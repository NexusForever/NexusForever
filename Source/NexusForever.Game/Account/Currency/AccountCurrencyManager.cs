using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.Account;
using NexusForever.Game.Abstract.Account.Currency;
using NexusForever.Game.Static.Account;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Account.Currency
{
    public class AccountCurrencyManager : IAccountCurrencyManager
    {
        private readonly Dictionary<AccountCurrencyType, IAccountCurrency> currencies = new();

        private readonly IAccount account;

        /// <summary>
        /// Create a new <see cref="IAccountCurrencyManager"/> from an existing database model.
        /// </summary>
        public AccountCurrencyManager(IAccount account, AccountModel model)
        {
            this.account = account;

            foreach (AccountCurrencyModel currencyModel in model.AccountCurrency)
            {
                // Disabled Character Token for now due to causing server errors if the player tries to use it. TODO: Fix level 50 creation
                if ((AccountCurrencyType)currencyModel.CurrencyId == AccountCurrencyType.MaxLevelToken)
                    continue;

                currencies.Add((AccountCurrencyType)currencyModel.CurrencyId, new AccountCurrency(account, currencyModel));
            }
        }

        public void Save(AuthContext context)
        {
            foreach (IAccountCurrency accountCurrency in currencies.Values)
                accountCurrency.Save(context);
        }

        /// <summary>
        /// Create a new <see cref="IAccountCurrency"/>.
        /// </summary>
        private IAccountCurrency CreateAccountCurrency(AccountCurrencyType currencyType, ulong amount = 0)
        {
            AccountCurrencyTypeEntry currencyEntry = GameTableManager.Instance.AccountCurrencyType.GetEntry((ulong)currencyType);
            if (currencyEntry == null)
                throw new ArgumentNullException($"AccountCurrencyTypeEntry not found for currencyId {currencyType}");

            if (currencies.TryAdd(currencyType, new AccountCurrency(account, currencyType, amount)))
                return currencies[currencyType];

            return null;
        }

        /// <summary>
        /// Returns whether the Account has enough of the currency to afford the amount.
        /// </summary>
        public bool CanAfford(AccountCurrencyType currencyType, ulong amount)
        {
            if (!currencies.TryGetValue(currencyType, out IAccountCurrency accountCurrency))
                return false;

            return accountCurrency.CanAfford(amount);
        }

        /// <summary>
        /// Add a supplied amount to an <see cref="IAccountCurrency"/>.
        /// </summary>
        public void CurrencyAddAmount(AccountCurrencyType currencyType, ulong amount, ulong reason = 0)
        {
            if (!currencies.TryGetValue(currencyType, out IAccountCurrency accountCurrency))
                accountCurrency = CreateAccountCurrency(currencyType, 0);

            if (accountCurrency == null)
                throw new ArgumentException($"Account Currency entry not found for currencyId {currencyType}.");

            accountCurrency.AddAmount(amount);
            SendAccountCurrencyUpdate(accountCurrency, reason);
        }

        /// <summary>
        /// Subtract a supplied amount from an <see cref="IAccountCurrency"/>.
        /// </summary>
        public void CurrencySubtractAmount(AccountCurrencyType currencyType, ulong amount, ulong reason = 0)
        {
            if (!currencies.TryGetValue(currencyType, out IAccountCurrency accountCurrency))
                accountCurrency = CreateAccountCurrency(currencyType, 0);

            if (accountCurrency == null)
                throw new ArgumentException($"Account Currency entry not found for currencyId {currencyType}.");

            if (!accountCurrency.CanAfford(amount))
                throw new ArgumentException($"Trying to remove more currency {accountCurrency.CurrencyId} than the player has!");

            // TODO: Ensure that we're not at cap - is there a cap?
            accountCurrency.SubtractAmount(amount);
            SendAccountCurrencyUpdate(accountCurrency, reason);
        }

        /// <summary>
        /// Sends information about all the player's <see cref="IAccountCurrency"/> during Character Select.
        /// </summary>
        public void SendCharacterListPacket()
        {
            account.Session.EnqueueMessageEncrypted(new ServerAccountCurrencySet
            {
                AccountCurrencies = currencies.Values.Select(c => c.Build()).ToList()
            });
        }

        /// <summary>
        /// Sends information about all the player's <see cref="IAccountCurrency"/> when entering world.
        /// </summary>
        public void SendInitialPackets()
        {
            foreach (IAccountCurrency accountCurrency in currencies.Values)
                SendAccountCurrencyUpdate(accountCurrency);
        }

        /// <summary>
        /// Sends information about a player's <see cref="AccountCurrency"/>.
        /// </summary>
        private void SendAccountCurrencyUpdate(IAccountCurrency accountCurrency, ulong reason = 0)
        {
            account.Session.EnqueueMessageEncrypted(new ServerAccountCurrencyGrant
            {
                AccountCurrency = accountCurrency.Build(),
                Unknown0 = reason
            });
        }
    }
}
