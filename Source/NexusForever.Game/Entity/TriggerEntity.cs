using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;

namespace NexusForever.Game.Entity
{
    public class TriggerEntity : WorldEntity, ITriggerEntity
    {
        public override EntityType Type => EntityType.Trigger;

        public TriggerEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        protected override IEntityModel BuildEntityModel()
        {
            throw new NotImplementedException();
        }
    }
}
