using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;

namespace NexusForever.Game.Entity
{
    internal class LockboxEntity : WorldEntity, ILockboxEntity
    {
        public override EntityType Type => EntityType.Lockbox;

        #region Dependency Injection

        public LockboxEntity(IMovementManager movementManager,
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
