using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    internal class SimpleCollidableEntity : WorldEntity, ISimpleCollidableEntity
    {
        public override EntityType Type => EntityType.SimpleCollidable;

        #region Dependency Injection

        public SimpleCollidableEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new SimpleCollidableEntityModel
            {
                CreatureId = CreatureId,
                QuestChecklistIdx = 0
            };
        }
    }
}
