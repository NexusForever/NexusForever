using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class InstancePortalEntity : WorldEntity, IInstancePortalEntity
    {
        public override EntityType Type => EntityType.InstancePortal;

        #region Dependency Injection

        public InstancePortalEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new InstancePortalEntityModel
            {
                CreatureId = CreatureId
            };
        }
    }
}
