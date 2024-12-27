using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class CameraEntity : WorldEntity, ICameraEntity
    {
        public override EntityType Type => EntityType.Camera;

        #region Dependency Injection

        public CameraEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new CameraEntityModel
            {
            };
        }
    }
}
