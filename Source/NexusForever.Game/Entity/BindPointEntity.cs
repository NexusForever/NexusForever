using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class BindPointEntity : WorldEntity, IBindPointEntity
    {
        public override EntityType Type => EntityType.BindPoint;

        #region Dependency Injection

        public BindPointEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new BindpointEntityModel
            {
                CreatureId = CreatureId
            };
        }
    }
}
