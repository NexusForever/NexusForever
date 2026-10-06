using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Loot.Generate
{
    public interface ILootGroupManager
    {
        void Initialise();

        /// <summary>
        /// Generates loot for the specified entity.
        /// </summary>
        /// <param name="id">The identifier of the entity to generate loot for.</param>
        /// <param name="players">The players eligible to receive the generated loot.</param>
        /// <returns>
        /// An <see cref="ILootBuilder"/> containing the generated loot, or <see langword="null"/> if loot generation failed.
        /// </returns>
        ILootBuilder GenerateEntityLoot(uint id, List<IPlayer> players);

        /// <summary>
        /// Generates loot for the specified item.
        /// </summary>
        /// <param name="id">The identifier of the item to generate loot for.</param>
        /// <param name="players">The players eligible to receive the generated loot.</param>
        /// <returns>
        /// An <see cref="ILootBuilder"/> containing the generated loot, or <see langword="null"/> if loot generation failed.
        ILootBuilder GenerateItemLoot(uint id, List<IPlayer> players);
    }
}
