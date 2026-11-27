﻿using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot.Generate;
using NexusForever.Game.Abstract.Loot.Generate.Condition;
using NexusForever.Game.Static.Loot;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Loot.Generate
{
    public class LootGroup : ILootGroup
    {
        public ulong Id { get; private set; }
        public float Probability { get; private set; }
        public uint MinCount { get; private set; }
        public uint MaxCount { get; private set; }
        public LootConditionType ConditionType { get; private set; }
        public uint Condition { get; private set; }

        private readonly List<ILootGenerator> lootGenerators = [];

        #region Dependency Injection

        private readonly IFactory<ILootGroup> lootGroupFactory;
        private readonly IFactory<ILootItem> lootItemFactory;
        private readonly ILootGenerationConditionManager lootGenerationConditionManager;
        private readonly IGameTableManager gameTableManager;

        public LootGroup(
            IFactory<ILootGroup> lootGroupFactory,
            IFactory<ILootItem> lootItemFactory,
            ILootGenerationConditionManager lootGenerationConditionManager,
            IGameTableManager gameTableManager)
        {
            this.lootGroupFactory               = lootGroupFactory;
            this.lootItemFactory                = lootItemFactory;
            this.lootGenerationConditionManager = lootGenerationConditionManager;
            this.gameTableManager               = gameTableManager;
        }

        #endregion

        /// <summary>
        /// Initiaise <see cref="ILootGroup"/> from specified model.
        /// </summary>
        /// <remarks>
        /// Also initialises any child loot groups owned by this loot group.
        /// </remarks>
        /// <param name="lootGroupModel">The model containing data for this loot group.</param>
        /// <param name="lootGroupBuilder">The builder used to create and initialise related loot groups.</param>
        public void Initialise(LootGroupModel lootGroupModel, ILootGroupBuilder lootGroupBuilder)
        {
            Id            = lootGroupModel.Id;
            Probability   = lootGroupModel.Probability;
            MinCount      = lootGroupModel.MinCount;
            MaxCount      = lootGroupModel.MaxCount;
            ConditionType = lootGroupModel.ConditionType;
            Condition     = lootGroupModel.Condition;

            foreach (LootGroupModel childLootGroupModel in lootGroupBuilder.GetLootGroupChildren(lootGroupModel.Id))
            {
                ILootGroup childLootGroup = lootGroupFactory.Resolve();
                childLootGroup.Initialise(childLootGroupModel, lootGroupBuilder);
                lootGenerators.Add(childLootGroup);
            }

            foreach (LootItemModel lootItemModel in lootGroupModel.Item)
            {
                ILootItem lootItem = lootItemFactory.Resolve();
                lootItem.Initialise(lootItemModel);
                lootGenerators.Add(lootItem);
            }
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
        /// <param name="source">The creature that supplied the loot, when present.</param>
        public void GenerateLoot(ILootBuilder lootBuilder, IEnumerable<IPlayer> players, IUnitEntity source = null)
        {
            if (!MeetsSourceCondition(source))
                return;

            List<IPlayer> eligiblePlayers = GetEligiblePlayers(players);
            if (eligiblePlayers.Count == 0)
                return;

            List<ILootGenerator> sourceLootGenerators = lootGenerators.Where(g => g.MeetsSourceCondition(source)).ToList();
            List<ILootGenerator> eligibleLootGenerators = sourceLootGenerators
                .Where(g => g.CanGenerateLoot())
                .ToList();

            if (MaxCount != 0 && eligibleLootGenerators.Count > MaxCount)
            {
                // If more than the maximum were selected, remove the lowest probability generators
                // until the maximum count is reached.
                IEnumerable<ILootGenerator> reducedLootGenerators = eligibleLootGenerators
                    .OrderBy(g => g.Probability)
                    .Take(eligibleLootGenerators.Count - (int)MaxCount);

                foreach (ILootGenerator lootGenerator in reducedLootGenerators)
                    eligibleLootGenerators.Remove(lootGenerator);
            }

            if (MinCount != 0 && eligibleLootGenerators.Count < MinCount)
            {
                // If fewer than the minimum were selected, fill the remainder with the highest probability
                // generators not already selected. The minimum count may still not be reached.
                IEnumerable<ILootGenerator> additionalLootGenerators = sourceLootGenerators
                    .Except(eligibleLootGenerators)
                    .OrderByDescending(g => g.Probability)
                    .Take((int)MinCount - eligibleLootGenerators.Count);

                eligibleLootGenerators.AddRange(additionalLootGenerators);
            }

            foreach (ILootGenerator lootGenerator in eligibleLootGenerators)
                lootGenerator.GenerateLoot(lootBuilder, eligiblePlayers, source);
        }

        public bool MeetsSourceCondition(IUnitEntity source)
        {
            if (ConditionType != LootConditionType.SourceWorldLocation)
                return true;

            if (source is not { IsAlive: false, EntityId: > 0u, Map.Entry: { }, Zone: { } })
                return false;

            WorldLocation2Entry location = gameTableManager.WorldLocation2.GetEntry(Condition);
            if (location == null || location.WorldId != source.Map.Entry.Id || location.WorldZoneId != source.Zone.Id)
                return false;

            var spawn = new Vector2(source.LeashPosition.X, source.LeashPosition.Z);
            var center = new Vector2(location.Position0, location.Position2);
            return Vector2.DistanceSquared(spawn, center) <= location.Radius * location.Radius;
        }

        private List<IPlayer> GetEligiblePlayers(IEnumerable<IPlayer> players)
        {
            List<IPlayer> eligiblePlayers = [];

            foreach (IPlayer player in players)
                if (PlayerMeetsConditions(player))
                    eligiblePlayers.Add(player);

            return eligiblePlayers;
        }

        private bool PlayerMeetsConditions(IPlayer player)
        {
            if (ConditionType is not (LootConditionType.None or LootConditionType.SourceWorldLocation))
            {
                bool result = lootGenerationConditionManager.Meets(player, ConditionType, Condition);
                if (!result)
                    return false;
            }

            return true;
        }
    }
}
