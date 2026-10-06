namespace NexusForever.Game.Abstract.Loot.Generate
{
    public interface ILootBuilder
    {
        /// <summary>
        /// Add a <see cref="ILootBuilderItem"/> to the loot builder.
        /// </summary>
        /// <param name="lootBuilderItem"><see cref="ILootBuilderItem"/> to add to the loot builder.</param>
        void AddLoot(ILootBuilderItem lootBuilderItem);

        /// <summary>
        /// Return all previously added <see cref="ILootBuilderItem"/>'s.
        /// </summary>
        IEnumerable<ILootBuilderItem> GetItems();
    }
}
