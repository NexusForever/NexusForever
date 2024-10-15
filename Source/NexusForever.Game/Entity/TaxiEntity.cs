using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class TaxiEntity : WorldEntity, ITaxiEntity
    {
        public override EntityType Type => EntityType.Taxi;

        #region Dependency Injection

        public TaxiEntity(IMovementManager movementManager,
            IEntitySummonFactory entitySummonFactory)
            : base(movementManager, entitySummonFactory)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new TaxiEntityModel
            {
                CreatureId = CreatureId
            };
        }
    }
}
