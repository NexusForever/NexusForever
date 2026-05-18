using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    internal class HousingMannequinEntity : WorldEntity, IHousingMannequinEntity
    {
        public override EntityType Type => EntityType.HousingMannequin;

        #region Dependency Injection

        public HousingMannequinEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new HousingMannequinEntityModel
            {
                CreatureId = CreatureId
            };
        }
    }
}
