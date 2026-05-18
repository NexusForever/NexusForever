using NexusForever.Game.Static.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IBuybackManager : IUpdate
    {
        /// <summary>
        /// Return stored <see cref="IBuybackItem"/> for <see cref="IPlayer"/> with unique id.
        /// </summary>
        IBuybackItem GetItem(IPlayer player, uint uniqueId);

        /// <summary>
        /// Create a new <see cref="IBuybackItem"/> from sold <see cref="IItem"/> for <see cref="IPlayer"/>.
        /// </summary>
        void AddItem(IPlayer player, IItem item, uint quantity, List<(CurrencyType CurrencyTypeId, ulong CurrencyAmount)> currencyChange);

        /// <summary>
        /// Remove stored <see cref="IBuybackItem"/> with supplied unique id for <see cref="IPlayer"/>.
        /// </summary>
        void RemoveItem(IPlayer player, IBuybackItem item);

        /// <summary>
        /// Send <see cref="ServerBuybackItems"/> for <see cref="IPlayer"/>.
        /// </summary>
        void SendBuybackItems(IPlayer player);
    }
}