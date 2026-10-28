using NexusForever.Database.World.Model;
using NexusForever.Game.Static.Loot;

namespace NexusForever.Game.Abstract.Loot.Generate
{
    public interface ILootGroup : ILootGenerator
    {
        ulong Id { get; }
        LootConditionType ConditionType { get; }
        uint Condition { get; }

        /// <summary>
        /// Initiaise <see cref="ILootGroup"/> from specified model.
        /// </summary>
        /// <remarks>
        /// Also initialises any child loot groups owned by this loot group.
        /// </remarks>
        /// <param name="lootGroupModel">The model containing data for this loot group.</param>
        /// <param name="lootGroupBuilder">The builder used to create and initialise related loot groups.</param>
        void Initialise(LootGroupModel lootGroupModel, ILootGroupBuilder lootGroupBuilder);
    }
}
