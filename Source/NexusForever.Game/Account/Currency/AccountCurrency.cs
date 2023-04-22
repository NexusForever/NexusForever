using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.Account;
using NexusForever.Game.Abstract.Account.Currency;
using NexusForever.Game.Static.Account;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using ServerAccountCurrency = NexusForever.Network.World.Message.Model.Shared.AccountCurrency;

namespace NexusForever.Game.Account.Currency
{
    public class AccountCurrency : IAccountCurrency
    {
        [Flags]
        public enum AccountCurrencySaveMask
        {
            None   = 0x0000,
            Create = 0x0001,
            Amount = 0x0002
        }

        public AccountCurrencyType CurrencyId { get; }
        public AccountCurrencyTypeEntry Entry { get; }

        public ulong Amount
        {
            get => amount;
            private set
            {
                amount += value;
                saveMask |= AccountCurrencySaveMask.Amount;
            }
        }

        private ulong amount;

        private AccountCurrencySaveMask saveMask;

        private readonly IAccount account;

        /// <summary>
        /// Create a new <see cref="IAccountCurrency"/> from an <see cref="AccountCurrencyModel"/>.
        /// </summary>
        public AccountCurrency(IAccount account, AccountCurrencyModel model)
        {
            this.account = account;
            CurrencyId   = (AccountCurrencyType)model.CurrencyId;
            Amount       = model.Amount;
            Entry        = GameTableManager.Instance.AccountCurrencyType.GetEntry((ulong)CurrencyId);

            saveMask = AccountCurrencySaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="IAccountCurrency"/>.
        /// </summary>
        public AccountCurrency(IAccount account, AccountCurrencyType currencyType, ulong amount)
        {
            this.account = account;
            CurrencyId   = currencyType;
            Amount       = amount;
            Entry        = GameTableManager.Instance.AccountCurrencyType.GetEntry((ulong)CurrencyId);

            saveMask = AccountCurrencySaveMask.Create;
        }

        public void Save(AuthContext context)
        {
            if (saveMask == AccountCurrencySaveMask.None)
                return;

            var model = new AccountCurrencyModel
            {
                Id         = account.Id,
                CurrencyId = (byte)CurrencyId,
                Amount     = Amount
            };

            if ((saveMask & AccountCurrencySaveMask.Create) != 0)
            {
                context.Add(model);
            }
            else if ((saveMask & AccountCurrencySaveMask.Amount) != 0)
            {
                EntityEntry<AccountCurrencyModel> entity = context.Attach(model);
                entity.Property(p => p.Amount).IsModified = true;
            }

            saveMask = AccountCurrencySaveMask.None;
        }

        /// <summary>
        /// Add a supplied amount to <see cref="IAccountCurrency"/>.
        /// </summary>
        public void AddAmount(ulong amount)
        {
            checked
            {
                Amount += amount;
            }
        }

        /// <summary>
        /// Subtract a supplied amount from <see cref="IAccountCurrency"/>.
        /// </summary>
        public void SubtractAmount(ulong amount)
        {
            checked
            {
                Amount -= amount;
            }
        }

        /// <summary>
        /// Returns if amount can be subtracted from <see cref="IAccountCurrency"/>.
        /// </summary>
        public bool CanAfford(ulong amount)
        {
            if (Amount < amount)
                return false;

            return true;
        }

        public ServerAccountCurrency Build()
        {
            return new ServerAccountCurrency
            {
                AccountCurrencyType = (byte)CurrencyId,
                Amount              = Amount
            };
        }
    }
}
