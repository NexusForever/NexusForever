using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    internal class DestructibleDoorEntity : WorldEntity, IDestructibleDoorEntity
    {
        public override EntityType Type => EntityType.DestructibleDoor;

        #region Dependency Injection

        public DestructibleDoorEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new DestructibleDoorEntityModel
            {
                CreatureId = CreatureId
            };
        }
    }
}
