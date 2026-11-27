﻿using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot.Generate;
using NexusForever.Game.Static.Loot;
using NexusForever.Shared;

namespace NexusForever.Game.Loot.Generate
{
    public class LootItem : ILootItem
    {
        public LootItemType Type { get; private set; }
        public uint StaticId { get; private set; }
        public float Probability { get; private set; }
        public uint MinCount { get; private set; }
        public uint MaxCount { get; private set; }

        #region Dependency Injection

        private readonly IFactory<ILootBuilderItem> lootGeneratorItemFactory;
        private bool isQuestStarter;

        public LootItem(
            IFactory<ILootBuilderItem> lootGeneratorItemFactory)
        {
            this.lootGeneratorItemFactory = lootGeneratorItemFactory;
        }

        #endregion

        /// <summary>
        /// Initiaise <see cref="ILootItem"/> from specified model.
        /// </summary>
        /// <param name="lootItemModel">The model containing data for this loot item.</param>
        /// <param name="isQuestStarter">Whether to recheck eligibility when the quest item is claimed.</param>
        public void Initialise(LootItemModel lootItemModel, bool isQuestStarter = false)
        {
            this.isQuestStarter = isQuestStarter;
            Type        = lootItemModel.Type;
            StaticId    = lootItemModel.StaticId;
            Probability = lootItemModel.Probability;
            MinCount    = lootItemModel.MinCount;
            MaxCount    = lootItemModel.MaxCount;
        }

        /// <summary>
        /// Determines whether this generator can generate loot.
        /// </summary>
        public bool CanGenerateLoot()
        {
            double roll = Random.Shared.NextDouble() * 100d;
            return Probability <= roll;
        }

        /// <summary>
        /// Generates loot and adds it to the specified loot builder.
        /// </summary>
        /// <param name="lootBuilder">The loot builder to add generated loot to.</param>
        /// <param name="players">The players eligible to receive the generated loot.</param>
        public void GenerateLoot(ILootBuilder lootBuilder, IEnumerable<IPlayer> players)
        {
            uint count = (uint)Random.Shared.Next((int)MinCount, (int)MaxCount);

            List<Identity> looters = players
                .Select(p => p.Identity)
                .ToList();

            ILootBuilderItem lootBuilderItem = lootGeneratorItemFactory.Resolve();
            lootBuilderItem.Initialise(Type, StaticId, count, looters);
            lootBuilderItem.IsQuestStarter = isQuestStarter;
            lootBuilder.AddLoot(lootBuilderItem);
        }
    }
}
