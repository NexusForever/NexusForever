using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;

namespace NexusForever.Game.Entity
{
    internal class PickupEntity : WorldEntity, IPickupEntity
    {
        public override EntityType Type => EntityType.Pickup;

        public PickupEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        protected override IEntityModel BuildEntityModel()
        {
            throw new NotImplementedException();
        }
    }
}
