using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Script.Template.Event
{
    public interface IEntityRandomMovementEvent : IScriptEvent
    {
        void Initialise(IWorldEntity owner, float range, float speed, SplineMode mode);
    }
}
