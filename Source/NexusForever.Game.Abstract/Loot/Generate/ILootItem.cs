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
        /// <param name="isQuestStarter">Whether to recheck eligibility when the quest item is claimed.</param>
        void Initialise(LootItemModel lootItemModel, bool isQuestStarter = false);
    }
}
