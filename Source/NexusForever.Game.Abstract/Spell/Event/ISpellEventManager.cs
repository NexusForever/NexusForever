using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Spell.Event
{
    public interface ISpellEventManager : IUpdate
    {
        bool HasPendingEvent { get; }

        void EnqueueEvent(ISpellEvent spellEvent);
        void CancelEvents();
    }
}
