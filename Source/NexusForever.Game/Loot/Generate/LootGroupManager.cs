﻿using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NexusForever.Database;
using NexusForever.Database.World;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot.Generate;
using NexusForever.Shared;

namespace NexusForever.Game.Loot.Generate
{
    public class LootGroupManager : ILootGroupManager
    {
        private readonly Dictionary<ulong, ILootGroup> lootGroups = [];
        private readonly Dictionary<uint, List<ILootGroup>> entityLoot = [];
        private readonly Dictionary<uint, List<ILootGroup>> itemLoot = [];

        #region Dependency Injection

        private readonly ILogger<LootGroupManager> log;
        private readonly IDatabaseManager databaseManager;
        private readonly IFactory<ILootGroup> lootGroupFactory;
        private readonly IFactory<ILootBuilder> lootBuilderFactory;

        public LootGroupManager(
            ILogger<LootGroupManager> log,
            IDatabaseManager databaseManager,
            IFactory<ILootGroup> lootGroupFactory,
            IFactory<ILootBuilder> lootBuilderFactory)
        {
            this.log                = log;
            this.databaseManager    = databaseManager;
            this.lootGroupFactory   = lootGroupFactory;
            this.lootBuilderFactory = lootBuilderFactory;
        }

        #endregion

        public void Initialise()
        {
            DateTime loadStarted = DateTime.Now;

            BuildLootGroups();

            foreach (ItemLootModel itemLootModel in databaseManager.GetDatabase<WorldDatabase>().GetItemLoot())
                BuildLoot(itemLootModel.Id, itemLootModel.LootGroupId, itemLoot);

            foreach (EntityLootModel entityLootModel in databaseManager.GetDatabase<WorldDatabase>().GetEntityLoot())
                BuildLoot(entityLootModel.Id, entityLootModel.LootGroupId, entityLoot);

            log.LogInformation($"Loaded LootGroups for {itemLoot.Count} Item(s) and {entityLoot.Count} Creature(s) in {(DateTime.Now - loadStarted).TotalMilliseconds}ms");
        }

        private void BuildLootGroups()
        {
            ImmutableList<LootGroupModel> lootGroupModels = databaseManager.GetDatabase<WorldDatabase>()
                .GetLootGroups();

            var builder = new LootGroupBuilder();
            builder.Initialise(lootGroupModels);

            foreach (LootGroupModel lootGroupModel in builder.GetRootLootGroups())
            {
                ILootGroup lootGroup = lootGroupFactory.Resolve();
                lootGroup.Initialise(lootGroupModel, builder);
                lootGroups.Add(lootGroup.Id, lootGroup);
            }
        }

        private void BuildLoot(uint id, ulong lootGroupId, Dictionary<uint, List<ILootGroup>> storage)
        {
            if (!lootGroups.TryGetValue(lootGroupId, out ILootGroup lootGroup))
                return;

            if (!storage.ContainsKey(id))
                storage.Add(id, []);

            storage[id].Add(lootGroup);
        }

        /// <summary>
        /// Generates loot for the specified entity.
        /// </summary>
        /// <param name="id">The identifier of the entity to generate loot for.</param>
        /// <param name="players">The players eligible to receive the generated loot.</param>
        /// <returns>
        /// An <see cref="ILootBuilder"/> containing the generated loot, or <see langword="null"/> if loot generation failed.
        /// </returns>
        public ILootBuilder GenerateEntityLoot(uint id, List<IPlayer> players)
        {
            return GenerateLoot(id, entityLoot, players);
        }

        /// <summary>
        /// Generates loot for the specified item.
        /// </summary>
        /// <param name="id">The identifier of the item to generate loot for.</param>
        /// <param name="players">The players eligible to receive the generated loot.</param>
        /// <returns>
        /// An <see cref="ILootBuilder"/> containing the generated loot, or <see langword="null"/> if loot generation failed.
        public ILootBuilder GenerateItemLoot(uint id, List<IPlayer> players)
        {
            return GenerateLoot(id, itemLoot, players);
        }

        private ILootBuilder GenerateLoot(uint id, Dictionary<uint, List<ILootGroup>> storage, IEnumerable<IPlayer> players)
        {
            if (!storage.TryGetValue(id, out List<ILootGroup> lootGroups))
                return null;

            ILootBuilder builder = lootBuilderFactory.Resolve();
            foreach (ILootGroup lootGroup in lootGroups)
                lootGroup.GenerateLoot(builder, players);

            return builder;
        }
    }
}
