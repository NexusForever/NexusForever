using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Static;
using System.Linq;

namespace NexusForever.Game.Entity
{
    public abstract class UnitEntity : WorldEntity, IUnitEntity
    {
        private readonly List<ISpell> pendingSpells = new();

        public float HitRadius { get; protected set; } = 1f;

        protected UnitEntity(EntityType type)
            : base(type)
        {
            InitialiseHitRadius();
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (ISpell spell in pendingSpells)
                spell.Dispose();
        }

        private void InitialiseHitRadius()
        {
            if (CreatureEntry == null)
                return;

            Creature2ModelInfoEntry modelInfoEntry = GameTableManager.Instance.Creature2ModelInfo.GetEntry(CreatureEntry.Creature2ModelInfoId);
            if (modelInfoEntry != null)
                HitRadius = modelInfoEntry.HitRadius * CreatureEntry.ModelScale;
        }

        public override void Update(double lastTick)
        {
            base.Update(lastTick);

            foreach (ISpell spell in pendingSpells.ToArray())
            {
                spell.Update(lastTick);
                spell.LateUpdate(lastTick);
                if (spell.IsFinished)
                    pendingSpells.Remove(spell);
            }
        }

        /// <summary>
        /// Cast a <see cref="ISpell"/> with the supplied spell4 id.
        /// </summary>
        /// <param name="spell4Id"></param>
        public void CastSpell(uint spell4Id)
        {
            CastSpell(spell4Id, new SpellParameters());
        }

        /// <summary>
        /// Cast a <see cref="ISpell"/> with the supplied spell id and <see cref="ISpellParameters"/>.
        /// </summary>
        public void CastSpell(uint spell4Id, ISpellParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException();

            Spell4Entry spell4Entry = GameTableManager.Instance.Spell4.GetEntry(spell4Id);
            if (spell4Entry == null)
                throw new ArgumentOutOfRangeException();

            CastSpell(spell4Entry.Spell4BaseIdBaseSpell, (byte)spell4Entry.TierIndex, parameters);
        }

        /// <summary>
        /// Cast a <see cref="ISpell"/> with the supplied spell base id, tier and <see cref="ISpellParameters"/>.
        /// </summary>
        public void CastSpell(uint spell4BaseId, byte tier, ISpellParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException();

            ISpellBaseInfo spellBaseInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(spell4BaseId);
            if (spellBaseInfo == null)
                throw new ArgumentOutOfRangeException();

            ISpellInfo spellInfo = spellBaseInfo.GetSpellInfo(tier);
            if (spellInfo == null)
                throw new ArgumentOutOfRangeException();

            parameters.SpellInfo = spellInfo;
            CastSpell(parameters);
        }

        /// <summary>
        /// Cast a <see cref="ISpell"/> with the supplied <see cref="ISpellParameters"/>.
        /// </summary>
        public void CastSpell(ISpellParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException();

            if (DisableManager.Instance.IsDisabled(DisableType.BaseSpell, parameters.SpellInfo.BaseInfo.Entry.Id))
            {
                if (this is IPlayer player)
                    player.SendSystemMessage($"Unable to cast base spell {parameters.SpellInfo.BaseInfo.Entry.Id} because it is disabled.");
                return;
            }

            if (DisableManager.Instance.IsDisabled(DisableType.Spell, parameters.SpellInfo.Entry.Id))
            {
                if (this is IPlayer player)
                    player.SendSystemMessage($"Unable to cast spell {parameters.SpellInfo.Entry.Id} because it is disabled.");
                return;
            }

            if (parameters.UserInitiatedSpellCast)
            {
                if (this is IPlayer player)
                    player.Dismount();
            }

            CastMethod castMethod = (CastMethod)parameters.SpellInfo.BaseInfo.Entry.CastMethod;
            if (parameters.ClientSideInteraction != null)
                castMethod = CastMethod.ClientSideInteraction;

            var spell = GlobalSpellManager.Instance.NewSpell(castMethod, this, parameters);
            if (!spell.Cast())
                return;

            // Don't store spell if it failed to initialise
            if (spell.IsFailed)
                return;

            pendingSpells.Add(spell);
        }

        /// <summary>
        /// Cancel any <see cref="ISpell"/>'s that are interrupted by movement.
        /// </summary>
        public void CancelSpellsOnMove()
        {
            foreach (ISpell spell in pendingSpells)
                if (spell.IsMovingInterrupted() && spell.IsCasting)
                    spell.CancelCast(CastResult.CasterMovement);
        }

        /// <summary>
        /// Cancel an <see cref="ISpell"/> based on its casting id.
        /// </summary>
        /// <param name="castingId">Casting ID of the spell to cancel</param>
        public void CancelSpellCast(uint castingId)
        {
            ISpell spell = pendingSpells.SingleOrDefault(s => s.CastingId == castingId);
            spell?.CancelCast(CastResult.SpellCancelled);
        }

        /// <summary>
        /// Checks if this <see cref="IUnitEntity"/> is currently casting a spell.
        /// </summary>
        /// <returns></returns>
        public bool IsCasting()
        {
            foreach (Spell.Spell spell in pendingSpells)
                if (spell.IsCasting)
                    return true;

            return false;
        }

        /// <summary>
        /// Check if this <see cref="IUnitEntity"/> has a spell active with the provided <see cref="Spell4Entry"/> Id
        /// </summary>
        public bool HasSpell(uint spell4Id, out ISpell spell, bool isCasting = false)
        {
            spell = pendingSpells.FirstOrDefault(i => i.IsCasting == isCasting && !i.IsFinished && i.Spell4Id == spell4Id);

            return spell != null;
        }

        /// <summary>
        /// Check if this <see cref="IUnitEntity"/> has a spell active with the provided <see cref="CastMethod"/>
        /// </summary>
        public bool HasSpell(CastMethod castMethod, out ISpell spell)
        {
            spell = pendingSpells.FirstOrDefault(i => !i.IsCasting && !i.IsFinished && i.CastMethod == castMethod);

            return spell != null;
        }

        /// <summary>
        /// Check if this <see cref="IUnitEntity"/> has a spell active with the provided <see cref="Func"/> predicate.
        /// </summary>
        public bool HasSpell(Func<ISpell, bool> predicate, out ISpell spell)
        {
            spell = pendingSpells.FirstOrDefault(predicate);

            return spell != null;
        }
    }
}
