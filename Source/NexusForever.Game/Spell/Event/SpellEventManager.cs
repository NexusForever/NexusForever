using NexusForever.Game.Abstract.Spell.Event;

namespace NexusForever.Game.Spell.Event
{
    public class SpellEventManager : ISpellEventManager
    {
        public bool HasPendingEvent => events.Count != 0;

        private readonly List<ISpellEvent> events = new();

        public void Update(double lastTick)
        {
            foreach (ISpellEvent spellEvent in events.ToArray())
            {
                spellEvent.Update(lastTick);
                if (spellEvent.Delay <= 0d)
                {
                    spellEvent.Callback.Invoke();
                    events.Remove(spellEvent);
                }
            }
        }

        public void EnqueueEvent(ISpellEvent spellEvent)
        {
            events.Add(spellEvent);
        }

        public void CancelEvents()
        {
            events.Clear();
        }
    }
}
