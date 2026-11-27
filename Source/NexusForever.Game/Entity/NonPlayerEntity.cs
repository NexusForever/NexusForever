using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Loot.Distribute;
using NexusForever.Game.Configuration.Model;
using NexusForever.Game.Map;
using NexusForever.Game.Map.Instance;
using NexusForever.Game.Static.Entity;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Game;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class NonPlayerEntity : CreatureEntity, INonPlayerEntity
    {
        public override EntityType Type => EntityType.NonPlayer;

        public IVendorInfo VendorInfo { get; private set; }

        private EntityModel staticSpawn;
        private UpdateTimer respawnTimer;
        private bool respawnQueued;

        #region Dependency Injection

        public NonPlayerEntity(
            IMovementManager movementManager,
            ILootDistributor lootDistributor)
            : base(movementManager, lootDistributor)
        {
        }

        #endregion

        public override void Initialise(EntityModel model)
        {
            base.Initialise(model);
            staticSpawn = model.Id == 0u ? null : model;

            if (model.EntityVendor != null)
            {
                CreateFlags |= EntityCreateFlag.HasInteractionPrereq;
                VendorInfo = new VendorInfo(model);
            }
        }

        protected override void OnDeath()
        {
            if (DeathState != null)
                return;

            base.OnDeath();
            if (staticSpawn == null || Map is not BaseMap || Map is MapInstance)
                return;

            double delay = SharedConfiguration.Instance.Get<MapConfig>().StaticCreatureRespawnSeconds;
            if (double.IsFinite(delay) && delay > 0d)
                respawnTimer = new UpdateTimer(delay);
        }

        public override void Update(double lastTick)
        {
            base.Update(lastTick);
            if (respawnTimer == null || Map is not BaseMap map)
                return;

            if (IsAlive)
            {
                respawnTimer = null;
                return;
            }

            respawnTimer.Update(lastTick);
            if (!respawnTimer.HasElapsed || map.LootManager.GetLootInstances(Guid).Any(instance => !instance.Expired))
                return;

            respawnTimer = null;
            respawnQueued = true;
            map.EnqueueCreatureRespawn(this, staticSpawn);
        }

        internal bool ConsumeRespawnRequest()
        {
            bool queued = respawnQueued;
            respawnQueued = false;
            return queued;
        }

        public override void OnEnqueueRemoveFromMap()
        {
            respawnTimer = null;
            respawnQueued = false;
            base.OnEnqueueRemoveFromMap();
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new NonPlayerEntityModel
            {
                CreatureId = CreatureId,
                QuestChecklistIdx = 0
            };
        }

        /// <summary>
        /// Calculate default property value for supplied <see cref="Property"/>.
        /// </summary>
        /// <remarks>
        /// Default property values are not sent to the client, they are also calculated by the client and are replaced by any property updates.
        /// </remarks>
        protected override float CalculateDefaultProperty(Property property)
        {
            float value = base.CalculateDefaultProperty(property);

            Creature2Entry creatureEntry = GameTableManager.Instance.Creature2.GetEntry(CreatureId);

            Creature2ArcheTypeEntry archeTypeEntry = GameTableManager.Instance.Creature2ArcheType.GetEntry(creatureEntry.Creature2ArcheTypeId);
            if (archeTypeEntry != null)
                value *= archeTypeEntry.UnitPropertyMultiplier[(uint)property];

            Creature2DifficultyEntry difficultyEntry = GameTableManager.Instance.Creature2Difficulty.GetEntry(creatureEntry.Creature2DifficultyId);
            if (difficultyEntry != null)
                value *= difficultyEntry.UnitPropertyMultiplier[(uint)property];

            Creature2TierEntry tierEntry = GameTableManager.Instance.Creature2Tier.GetEntry(creatureEntry.Creature2TierId);
            if (tierEntry != null)
                value *= tierEntry.UnitPropertyMultiplier[(uint)property];

            return value;
        }
    }
}
