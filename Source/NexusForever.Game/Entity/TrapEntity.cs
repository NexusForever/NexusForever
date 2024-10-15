using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;

namespace NexusForever.Game.Entity
{
    public class TrapEntity : WorldEntity, ITrapEntity
    {
        public override EntityType Type => EntityType.Trap;

        #region Dependency Injection

        public TrapEntity(IMovementManager movementManager,
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
