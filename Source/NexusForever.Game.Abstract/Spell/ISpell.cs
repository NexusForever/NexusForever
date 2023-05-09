using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Spell
{
    public interface ISpell : IDisposable, IUpdate
    {
        ISpellParameters Parameters { get; }
        IUnitEntity Caster { get; }

        uint CastingId { get; }
        uint Spell4Id { get; }
        CastMethod CastMethod { get; }

        bool IsCasting { get; }
        bool IsFinished { get; }
        bool IsFailed { get; }
        bool IsWaiting { get; }

        bool HasGroup(uint groupId);

        /// <summary>
        /// Begin cast, checking prerequisites before initiating.
        /// </summary>
        bool Cast();

        /// <summary>
        /// Invoked each world tick, after Update() for this <see cref="ISpell"/>, with the delta since the previous tick occurred.
        /// </summary>
        void LateUpdate(double lastTick);

        /// <summary>
        /// Cancel cast with supplied <see cref="CastResult"/>.
        /// </summary>
        void CancelCast(CastResult result);

        bool IsMovingInterrupted();

        /// <summary>
        /// Add a <see cref="IProxy"/> to this spell's execution queue.
        /// </summary>
        /// <param name="proxy">Proxy instance to add</param>
        void AddProxy(IProxy proxy);

        /// <summary>
        /// Returns number of times a certain effect has been triggered, for this spell cast, with a given ID.
        /// </summary>
        /// <param name="effectId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool GetEffectTriggerCount(uint effectId, out uint count);
    }
}