using NexusForever.Database.Auth;
using NexusForever.Game.Static.Account;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Account.Currency
{
    public interface IAccountCurrency : IDatabaseAuth, INetworkBuildable<AccountCurrency>
    {
        AccountCurrencyType CurrencyId { get; }
        AccountCurrencyTypeEntry Entry { get; }
        ulong Amount { get; }

        /// <summary>
        /// Add a supplied amount to <see cref="IAccountCurrency"/>.
        /// </summary>
        void AddAmount(ulong amount);

        /// <summary>
        /// Subtract a supplied amount from <see cref="IAccountCurrency"/>.
        /// </summary>
        void SubtractAmount(ulong amount);

        /// <summary>
        /// Returns if amount can be subtracted from <see cref="IAccountCurrency"/>.
        /// </summary>
        bool CanAfford(ulong amount);
    }
}