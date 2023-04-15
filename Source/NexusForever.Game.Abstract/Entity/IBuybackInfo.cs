using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IBuybackInfo : IEnumerable<IBuybackItem>
    {
        /// <summary>
        /// Return stored <see cref="IBuybackItem"/> with supplied unique id.
        /// </summary>
        IBuybackItem GetItem(uint uniqueId);

        /// <summary>
        /// Create a new <see cref="IBuybackItem"/> from sold <see cref="IItem"/>.
        /// </summary>
        uint AddItem(IItem item, uint quantity, List<(CurrencyType CurrencyTypeId, ulong CurrencyAmount)> currencyChange);

        /// <summary>
        /// Remove stored <see cref="IBuybackItem"/> with supplied unique id.
        /// </summary>
        void RemoveItem(uint uniqueId);
    }
}