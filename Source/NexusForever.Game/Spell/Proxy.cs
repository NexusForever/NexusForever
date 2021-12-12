using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Event;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Spell.Event;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Spell
{
    public class Proxy : IProxy
    {
        public IUnitEntity Target { get; }
        public Spell4EffectsEntry Entry { get; }
        public ISpell ParentSpell { get; }
        public bool CanCast { get; private set; } = false;

        private ISpellParameters proxyParameters;

        public Proxy(IUnitEntity target, Spell4EffectsEntry entry, ISpell parentSpell, ISpellParameters parameters)
        {
            Target = target;
            Entry = entry;
            ParentSpell = parentSpell;

            proxyParameters = new SpellParameters
            {
                ParentSpellInfo = parameters.SpellInfo,
                RootSpellInfo = parameters.RootSpellInfo,
                PrimaryTargetId = Target.Guid,
                UserInitiatedSpellCast = parameters.UserInitiatedSpellCast,
                IsProxy = true
            };
        }

        public void Evaluate()
        {
            if (Target is not IPlayer)
                CanCast = true;

            if (Entry.DataBits06 == 0)
                CanCast = true;

            if (CanCast)
                return;

            if (PrerequisiteManager.Instance.Meets(Target as IPlayer, Entry.DataBits06))
                CanCast = true;
        }

        public void Cast(IUnitEntity caster, ISpellEventManager events)
        {
            if (!CanCast)
                return;

            if (ParentSpell.CastMethod == CastMethod.Aura && Entry.TickTime > 0)
            {
                caster.CastSpell(Entry.DataBits01, proxyParameters);
                return;
            }

            events.EnqueueEvent(new SpellEvent(Entry.DelayTime / 1000d, () =>
            {
                if (Entry.TickTime > 0)
                {
                    double tickTime = Entry.TickTime;
                    if (Entry.DurationTime > 0)
                    {
                        for (int i = 1; i >= Entry.DurationTime / tickTime; i++)
                            events.EnqueueEvent(new SpellEvent(tickTime * i / 1000d, () =>
                            {
                                caster.CastSpell(Entry.DataBits01, proxyParameters);
                            }));
                    }
                    else
                        events.EnqueueEvent(TickingEvent(tickTime, () =>
                        {
                            caster.CastSpell(Entry.DataBits01, proxyParameters);
                        }));
                }
                else
                    caster.CastSpell(Entry.DataBits00, proxyParameters);
            }));
        }

        private SpellEvent TickingEvent(double tickTime, Action action)
        {
            return new SpellEvent(tickTime / 1000d, () =>
            {
                action.Invoke();
                TickingEvent(tickTime, action);
            });
        }
    }
}