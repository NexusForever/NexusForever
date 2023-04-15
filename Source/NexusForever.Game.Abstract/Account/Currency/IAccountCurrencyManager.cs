using NexusForever.Database.Auth;
using NexusForever.Game.Static.Account;

namespace NexusForever.Game.Abstract.Account.Currency
{
    public interface IAccountCurrencyManager : IDatabaseAuth
    {
        /// <summary>
        /// Returns whether the Account has enough of the currency to afford the amount.
        /// </summary>
        bool CanAfford(AccountCurrencyType currencyType, ulong amount);

        /// <summary>
        /// Add a supplied amount to an <see cref="IAccountCurrency"/>.
        /// </summary>
        void CurrencyAddAmount(AccountCurrencyType currencyType, ulong amount, ulong reason = 0);

        /// <summary>
        /// Subtract a supplied amount from an <see cref="IAccountCurrency"/>.
        /// </summary>
        void CurrencySubtractAmount(AccountCurrencyType currencyType, ulong amount, ulong reason = 0);

        /// <summary>
        /// Sends information about all the player's <see cref="IAccountCurrency"/> during character select.
        /// </summary>
        void SendCharacterListPacket();

        /// <summary>
        /// Sends information about a player's <see cref="IAccountCurrency"/>.
        /// </summary>
        void SendInitialPackets();
    }
}