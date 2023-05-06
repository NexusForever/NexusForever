using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement.Generator;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Script.Template.Event
{
    public class EntityRandomMovementEvent : IEntityRandomMovementEvent
    {
        private IWorldEntity owner;
        private float range;
        private float speed;
        private SplineMode mode;

        #region Dependency Injection

        private readonly IRandomMovementGenerator randomMovementGenerator;

        public EntityRandomMovementEvent(
            IRandomMovementGenerator randomMovementGenerator)
        {
            this.randomMovementGenerator = randomMovementGenerator;
        }

        #endregion

        public void Initialise(IWorldEntity owner, float range, float speed, SplineMode mode)
        {
            this.owner = owner;
            this.range = range;
            this.speed = speed;
            this.mode  = mode;
        }

        public void Invoke()
        {
            randomMovementGenerator.Map   = owner.Map;
            randomMovementGenerator.Begin = owner.Position;
            randomMovementGenerator.Leash = owner.LeashPosition;
            randomMovementGenerator.Range = range;
            owner.MovementManager.LaunchGenerator(randomMovementGenerator, speed, mode);
        }
    }
}
