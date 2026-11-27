using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Database.World.Model;

namespace NexusForever.Game.Entity
{
    public class CollectableUnitEntity : WorldEntity, ICollectableUnitEntity
    {
        public override EntityType Type => EntityType.CollectableUnit;
        public byte QuestChecklistIdx { get; private set; }

        #region Dependency Injection

        public CollectableUnitEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        public override void Initialise(EntityModel model)
        {
            base.Initialise(model);
            if (model is { World: Quest.FarsideExpeditionSupplies.World, Area: Quest.FarsideExpeditionSupplies.Ravine,
                    Creature: Quest.FarsideExpeditionSupplies.SupplyCrate })
                QuestChecklistIdx = model.QuestChecklistIdx;
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new CollectableUnitEntityModel
            {
                CreatureId = CreatureId,
                QuestChecklistIdx = QuestChecklistIdx
            };
        }
    }
}
