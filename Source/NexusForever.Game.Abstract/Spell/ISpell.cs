using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Static.Spell;
using NexusForever.Network.World.Message.Model.Spell;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Spell
{
    public interface ISpell : IDisposable, IUpdate
    {
        CastMethod CastMethod { get; }

        ISpellParameters Parameters { get; }
        IUnitEntity Caster { get; }
        uint CastingId { get; }
        uint Spell4Id { get; }

        bool IsCasting { get; }
        bool IsFinished { get; }
        bool IsFailed { get; }
        bool IsWaiting { get; }


        /// <summary>
        /// Initialise <see cref="ISpell"/> with the supplied <see cref="IUnitEntity"/> and <see cref="ISpellParameters"/>.
        /// </summary>
        void Initialise(IUnitEntity caster, ISpellParameters parameters);

        /// <summary>
        /// Invoked each world tick, after Update() for this <see cref="ISpell"/>, with the delta since the previous tick occurred.
        /// </summary>
        void LateUpdate(double lastTick);

        /// <summary>
        /// Return <see cref="ISpellTargetInfo"/> for the supplied <see cref="IUnitEntity"/>.
        /// </summary>
        ISpellTargetInfo GetTarget(IUnitEntity entity);

        /// <summary>
        /// Begin cast, checking prerequisites before initiating.
        /// </summary>
        bool Cast();

        /// <summary>
        /// Cancel cast with supplied <see cref="CastResult"/>.
        /// </summary>
        void CancelCast(CastResult result);

        /// <summary>
        /// Finish this <see cref="ISpell"/> and end all effects associated with it.
        /// </summary>
        void Finish();

        bool IsMovingInterrupted();

        SpellInit BuildSpellInit();
    }
}
