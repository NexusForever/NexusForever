using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ICurrencyManager : IDatabaseCharacter, IEnumerable<ICurrency>
    {
        /// <summary>
        /// Returns total amount of acquired <see cref="CurrencyType"/>.
        /// </summary>
        ulong? GetCurrency(CurrencyType currencyId);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> has the supplied amount of <see cref="CurrencyType"/>.
        /// </summary>
        bool CanAfford(CurrencyType currencyId, ulong amount);

        /// <summary>
        /// Add supplied amount of <see cref="CurrencyType"/> currency.
        /// </summary>
        void CurrencyAddAmount(CurrencyType currencyId, ulong amount, bool isLoot = false);

        /// <summary>
        /// Remove supplied amount of <see cref="CurrencyType"/> currency.
        /// </summary>
        void CurrencySubtractAmount(CurrencyType currencyId, ulong amount, bool isLoot = false);
    }
}