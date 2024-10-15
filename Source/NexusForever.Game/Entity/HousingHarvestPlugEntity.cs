using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;

namespace NexusForever.Game.Entity
{
    internal class HousingHarvestPlugEntity : WorldEntity, IHousingHarvestPlugEntity
    {
        public override EntityType Type => EntityType.HousingHarvestPlug;

        #region Dependency Injection

        public HousingHarvestPlugEntity(IMovementManager movementManager,
            IEntitySummonFactory entitySummonFactory)
            : base(movementManager, entitySummonFactory)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            throw new NotImplementedException();
        }
    }
}
