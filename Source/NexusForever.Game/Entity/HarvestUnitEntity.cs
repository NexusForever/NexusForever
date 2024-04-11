using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class HarvestUnitEntity : WorldEntity, IHarvestUnitEntity
    {
        public override EntityType Type => EntityType.HarvestUnit;

        #region Dependency Injection

        public HarvestUnitEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new HarvestUnitEntityModel
            {
                CreatureId = CreatureId
            };
        }
    }
}
