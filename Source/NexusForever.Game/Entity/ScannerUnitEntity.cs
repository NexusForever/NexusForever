using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;

namespace NexusForever.Game.Entity
{
    public class ScannerUnitEntity : WorldEntity, IScannerUnitEntity
    {
        public override EntityType Type => EntityType.ScannerUnit;

        #region Dependency Injection

        public ScannerUnitEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            throw new NotImplementedException();
        }
    }
}
