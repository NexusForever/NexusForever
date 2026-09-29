using NexusForever.Game.Abstract.Spell.Event;
using NLog;

namespace NexusForever.Game.Spell.Event
{
    public class SpellEventManager : ISpellEventManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public bool HasPendingEvent => events.Count != 0;

        private readonly List<ISpellEvent> events = new();

        public void Update(double lastTick)
        {
            foreach (ISpellEvent spellEvent in events.ToArray())
            {
                spellEvent.Update(lastTick);
                if (spellEvent.Delay <= 0d)
                {
                    // Always dequeue first: a throwing callback must not re-fire every tick
                    // (that permanently locks the caster in SpellStatus.Executing).
                    events.Remove(spellEvent);
                    try
                    {
                        spellEvent.Callback.Invoke();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, "Unhandled exception during spell event callback.");
                    }
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
