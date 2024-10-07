using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Event;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class CollectableUnitEntity : UnitEntity, ICollectableUnitEntity
    {
        public override EntityType Type => EntityType.CollectableUnit;

        public byte QuestChecklistIdx { get; private set; }

        #region Dependency Injection

        private readonly IAssetManager assetManager;

        public CollectableUnitEntity(
            IMovementManager movementManager,
            IAssetManager assetManager)
            : base(movementManager)
        {
            this.assetManager = assetManager;
        }

        #endregion

        public override void Initialise(EntityModel model)
        {
            base.Initialise(model);
            QuestChecklistIdx = model.QuestChecklistIdx;
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new CollectableUnitEntityModel
            {
                CreatureId        = CreatureId,
                QuestChecklistIdx = QuestChecklistIdx
            };
        }

        public override void OnActivateSuccess(IPlayer activator)
        {
            foreach (uint targetGroupId in AssetManager.Instance.GetTargetGroupsForCreatureId(CreatureId) ?? Enumerable.Empty<uint>())
                Map.PublicEventManager.UpdateObjective(activator, PublicEventObjectiveType.ActivateTargetGroupChecklist, targetGroupId, QuestChecklistIdx);
        }
    }
}
