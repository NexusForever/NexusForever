using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class ResidenceEntity : WorldEntity, IResidenceEntity
    {
        public override EntityType Type => EntityType.Residence;

        #region Dependency Injection

        public ResidenceEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new ResidenceEntityModel
            {
                CreatureId = CreatureId
            };
        }
    }
}
