using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    internal class WorldUnitEntity : WorldEntity, IWorldUnitEntity
    {
        public override EntityType Type => EntityType.WorldUnit;

        #region Dependency Injection

        public WorldUnitEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new WorldUnitEntityModel
            {
            };
        }
    }
}
