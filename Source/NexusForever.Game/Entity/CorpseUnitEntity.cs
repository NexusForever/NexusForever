using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class CorpseUnitEntity : WorldEntity, ICorpseUnitEntity
    {
        public override EntityType Type => EntityType.CorpseUnit;

        #region Dependency Injection

        public CorpseUnitEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new CorpseEntityModel
            {
                CreatureId = CreatureId
            };
        }
    }
}
