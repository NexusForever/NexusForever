using NexusForever.Game.Abstract.Entity;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Quest
{
    public abstract class QuestActorActivation
    {
        internal abstract IWorldEntity Target { get; }
        internal abstract Spell4EffectsEntry ActivationEffect { get; }
        internal abstract double CastSeconds { get; }

        internal abstract bool CanFinish();
        internal abstract void Complete();
    }
}
