using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Creature;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Stat;
using NexusForever.Game.Abstract.Loot.Distribute;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Script;
using NexusForever.Script.Template.Collection;

namespace NexusForever.Game.Entity
{
    public class NonPlayerEntity : CreatureEntity, INonPlayerEntity
    {
        public override EntityType Type => EntityType.NonPlayer;

        public IVendorInfo VendorInfo { get; private set; }

        #region Dependency Injection

        public NonPlayerEntity(
            IMovementManager movementManager,
            IEntitySummonFactory entitySummonFactory,
            IStatUpdateManager<IUnitEntity> statUpdateManager,
            ISpellFactory spellFactory,
            ILootDistributor lootDistributor)
            : base(movementManager, entitySummonFactory, statUpdateManager, spellFactory, lootDistributor)
        {
            statUpdateManager.Initialise(this);
        }

        #endregion

        public override void Initialise(ICreatureInfo creatureInfo, EntityModel model)
        {
            base.Initialise(creatureInfo, model);

            if (model.EntityVendor != null)
            {
                CreateFlags |= EntityCreateFlag.HasInteractionPrereq;
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

            if (CreatureInfo.ArcheTypeEntry != null)
                value *= CreatureInfo.ArcheTypeEntry.UnitPropertyMultiplier[(uint)property];

            if (CreatureInfo.DifficultyEntry != null)
                value *= CreatureInfo.DifficultyEntry.UnitPropertyMultiplier[(uint)property];

            if (CreatureInfo.TierEntry != null)
                value *= CreatureInfo.TierEntry.UnitPropertyMultiplier[(uint)property];

            return value;
        }
    }
}
