using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot.Generate;
using NexusForever.Game.Static.Loot;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Loot
{
    public interface ILootInstance : IUpdate
    {
        ILootManager LootManager { get; }
        uint OwnerGuid { get; }
        bool PersonalLoot { get; }
        bool Expired { get; }

        void Initialise(ILootManager lootManager, IUnitEntity entity);

        /// <summary>
        /// Gets all loot items associated with this loot instance.
        /// </summary>
        /// <returns>
        /// The loot items associated with this loot instance, or an empty collection if none exist.
        /// </returns>
        IEnumerable<ILootInstanceItem> GetLootItems();

        /// <summary>
        /// Gets the loot item instance for the specified loot id.
        /// </summary>
        /// <param name="lootId">The id of the loot item to retrieve.</param>
        /// <returns>
        /// The loot item instance, or null if not found.
        /// </returns>
        ILootInstanceItem GetLootItem(uint lootId);

        /// <summary>
        /// Adds a loot item to this loot instance.
        /// </summary>
        /// <param name="lootBuilderItem">The loot builder used to create the loot item.</param>
        /// <returns>
        /// The created loot item instance.
        /// </returns>
        ILootInstanceItem AddLootItem(ILootBuilderItem lootBuilderItem);

        /// <summary>
        /// Adds a loot item to this loot instance.
        /// </summary>
        /// <param name="staticId">The static id of the loot item, this will vary depending on the <paramref name="type"/>.</param>
        /// <param name="type">The type of the loot item.</param>
        /// <param name="count">The count of the loot item.</param>
        /// <param name="looters">The list of players who can loot the item.</param>
        /// <returns>
        /// The created loot item instance.
        /// </returns>
        ILootInstanceItem AddLootItem(uint staticId, LootItemType type, uint count, IEnumerable<Identity> looters);

        /// <summary>
        /// Sends a loot notification to all players who can loot the items from this loot instance.
        /// </summary>
        void SendLootNotify();

        /// <summary>
        /// Sends a loot removal message to all players who can loot the items from this loot instance.
        /// </summary>
        void SendLootRemove();
    }
}
