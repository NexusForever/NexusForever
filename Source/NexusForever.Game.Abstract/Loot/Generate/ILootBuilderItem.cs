using NexusForever.Game.Static.Loot;

namespace NexusForever.Game.Abstract.Loot.Generate
{
    public interface ILootBuilderItem
    {
        LootItemType Type { get; }
        uint Id { get; }
        uint Count { get; }
        List<Identity> Looters { get; }

        /// <summary>
        /// Initialise <see cref="ILootBuilderItem"/>.
        /// </summary>
        /// <param name="type">The type of loot.</param>
        /// <param name="id">The identifier of the loot, specific to the specified <paramref name="type"/>.</param>
        /// <param name="count">The quantity of this loot item to drop.</param>
        /// <param name="looters">The player identities eligible to loot this item.</param>
        void Initialise(LootItemType type, uint id, uint count, List<Identity> looters);
    }
}
