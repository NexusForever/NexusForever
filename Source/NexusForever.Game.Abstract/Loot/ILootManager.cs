using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Abstract.Map;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Loot
{
    public interface ILootManager : IUpdate
    {
        IBaseMap Map { get; }

        void Initialise(IBaseMap map);

        /// <summary>
        /// Gets all loot instances associated with the specified unit.
        /// </summary>
        /// <param name="ownerUnitId">The id of the unit that owns the loot instances.</param>
        /// <returns>
        /// The loot instances associated with the specified unit, or an empty collection if none exist.
        /// </returns>
        IEnumerable<ILootInstance> GetLootInstances(uint ownerUnitId);

        /// <summary>
        /// Get the loot item instance for the specified owner unit and loot id.
        /// </summary>
        /// <param name="ownerUnitId">The id of the unit that owns the loot instances.</param>
        /// <param name="lootId">The id of the loot item to retrieve.</param>
        /// <returns>
        /// The loot item instance, or null if not found.
        /// </returns>
        ILootInstanceItem GetLootItemInstance(uint ownerUnitId, uint lootId);

        /// <summary>
        /// Generates loot for the specified player from a looted item.
        /// </summary>
        /// <param name="looter">The player who looted the item.</param>
        /// <param name="lootedItem">The item that was looted.</param>
        void GenerateLoot(IPlayer looter, IItem lootedItem);

        /// <summary>
        /// Generates loot for the specified player from a looted entity.
        /// </summary>
        /// <param name="looter">The player who looted the entity.</param>
        /// <param name="lootedEntity">The entity that was looted.</param>
        void GenerateLoot(IPlayer looter, IUnitEntity lootedEntity);

        /// <summary>
        /// Generates loot for the specified group from a looted entity.
        /// </summary>
        /// <param name="group">The group that looted the entity.</param>
        /// <param name="lootedEntity">The entity that was looted.</param>
        void GenerateLoot(IGroup group, IUnitEntity lootedEntity);

        /// <summary>
        /// Vacuums all loot in range for the specified player.
        /// </summary>
        /// <param name="player">The player to vacuum loot for.</param>
        void VacuumLoot(IPlayer player);
    }
}
