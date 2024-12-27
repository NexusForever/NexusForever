using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Entity
{
    public class CollectableUnitEntity : WorldEntity, ICollectableUnitEntity
    {
        public override EntityType Type => EntityType.CollectableUnit;

        #region Dependency Injection

        public CollectableUnitEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new CollectableUnitEntityModel
            {
                CreatureId = CreatureId,
                QuestChecklistIdx = 0
            };
        }
    }
}
