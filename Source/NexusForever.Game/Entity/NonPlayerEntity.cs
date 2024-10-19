using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Script.Template.Collection;
using NexusForever.Script;

namespace NexusForever.Game.Entity
{
    public class NonPlayerEntity : CreatureEntity, INonPlayerEntity
    {
        public override EntityType Type => EntityType.NonPlayer;

        public IVendorInfo VendorInfo { get; private set; }

        #region Dependency Injection

        public NonPlayerEntity(
            IMovementManager movementManager,
            IEntitySummonFactory entitySummonFactory)
            : base(movementManager, entitySummonFactory)
        {
        }

        #endregion

        public override void Initialise(EntityModel model)
        {
            base.Initialise(model);

            if (model.EntityVendor != null)
            {
                CreateFlags |= EntityCreateFlag.Vendor;
                VendorInfo = new VendorInfo(model);
            }
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new NonPlayerEntityModel
            {
                CreatureId        = CreatureId,
                QuestChecklistIdx = QuestChecklistIdx
            };
        }

        /// <summary>
        /// Initialise <see cref="IScriptCollection"/> for <see cref="INonPlayerEntity"/>.
        /// </summary>
        protected override void InitialiseScriptCollection(List<string> names)
        {
            scriptCollection = ScriptManager.Instance.InitialiseOwnedCollection<INonPlayerEntity>(this);
            ScriptManager.Instance.InitialiseEntityScripts<INonPlayerEntity>(scriptCollection, this, names);
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
