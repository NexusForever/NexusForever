using NexusForever.Database.World.Model;
using NexusForever.Game.Static.Loot;

namespace NexusForever.Game.Abstract.Loot.Generate
{
    public interface ILootItem : ILootGenerator
    {
        LootItemType Type { get; }

        uint StaticId { get; }

        /// <summary>
        /// Initiaise <see cref="ILootItem"/> from specified model.
        /// </summary>
        /// <param name="lootItemModel">The model containing data for this loot item.</param>
        void Initialise(LootItemModel lootItemModel);
    }
}
