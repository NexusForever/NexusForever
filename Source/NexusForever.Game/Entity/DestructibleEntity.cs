using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Entity
{
    public class DestructibleEntity : WorldEntity, IDestructibleEntity
    {
        public override EntityType Type => EntityType.Destructible;

        #region Dependency Injection

        public DestructibleEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new DestructibleEntityModel
            {
                CreatureId = CreatureId
            };
        }
    }
}
