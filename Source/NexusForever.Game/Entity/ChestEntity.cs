using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class ChestEntity : WorldEntity, IChestEntity
    {
        public override EntityType Type => EntityType.Chest;

        #region Dependency Injection

        public ChestEntity(IMovementManager movementManager,
            IEntitySummonFactory entitySummonFactory)
            : base(movementManager, entitySummonFactory)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new ChestEntityModel
            {
                CreatureId = CreatureId
            };
        }
    }
}
