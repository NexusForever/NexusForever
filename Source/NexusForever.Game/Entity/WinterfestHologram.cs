using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Entity
{
    internal sealed class WinterfestHologram(IMovementManager movementManager) : SimpleEntity(movementManager)
    {
        internal IPlayer Owner { get; private set; }
        private double remaining;

        internal void Prepare(IPlayer owner, uint creature, double duration)
        {
            Owner = owner;
            remaining = duration;
            Initialise(new EntityModel { Type = EntityType.Simple, Creature = creature, DisplayInfo = 36478, Faction1 = 219, Faction2 = 219 });
        }

        public override void Update(double lastTick)
        {
            base.Update(lastTick);
            if (remaining <= 0d)
                return;

            remaining -= lastTick;
            if (remaining <= 0d || Owner.Map != Map)
            {
                remaining = 0d;
                Map?.EnqueueRemove(this);
            }
        }
    }
}
