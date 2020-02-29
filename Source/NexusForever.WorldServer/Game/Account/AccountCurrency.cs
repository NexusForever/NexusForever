using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Account.Static;
using ServerAccountCurrency = NexusForever.WorldServer.Network.Message.Model.Shared.AccountCurrency;

namespace NexusForever.WorldServer.Game.Account
{
    public class AccountCurrency : IBuildable<ServerAccountCurrency>
    {
        public AccountCurrencyType CurrencyId { get; }
        public AccountCurrencyTypeEntry Entry { get; }
        public ulong Amount { get; private set; }

        private readonly uint accountId;
        private AccountCurrencySaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="AccountCurrency"/> from an <see cref="AccountCurrencyModel"/>
        /// </summary>
        public AccountCurrency(AccountCurrencyModel model)
        {
            accountId   = model.Id;
            CurrencyId  = (AccountCurrencyType)model.CurrencyId;
            Amount      = model.Amount;
            Entry       = GameTableManager.Instance.AccountCurrencyType.GetEntry((ulong)CurrencyId);

            saveMask = AccountCurrencySaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="AccountCurrency"/>
        /// </summary>
        public AccountCurrency(uint accountId, AccountCurrencyType currencyType, ulong amount)
        {
            this.accountId  = accountId;
            CurrencyId      = currencyType;
            Amount          = amount;
            Entry           = GameTableManager.Instance.AccountCurrencyType.GetEntry((ulong)CurrencyId);

            saveMask = AccountCurrencySaveMask.Create;
        }

        public void Save(AuthContext context)
        {
            if (saveMask == AccountCurrencySaveMask.None)
                return;

            var model = new AccountCurrencyModel
            {
                Id = accountId,
                CurrencyId = (byte)CurrencyId,
                Amount = Amount
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

        public bool AddAmount(ulong amount)
        {
            Amount += amount;
            saveMask |= AccountCurrencySaveMask.Amount;

            return true;
        }

        public bool SubtractAmount(ulong amount)
        {
            if (!CanAfford(amount))
                return false;

            Amount -= amount;
            saveMask |= AccountCurrencySaveMask.Amount;

            return true;
        }

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
