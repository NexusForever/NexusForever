using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Spell.Event
{
    public interface ISpellEvent : IUpdate
    {
        double Delay { get; }
        Action Callback { get; }
    }
}
