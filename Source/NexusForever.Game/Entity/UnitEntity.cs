using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Static;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.Game.Entity
{
    public abstract class UnitEntity : WorldEntity, IUnitEntity
    {
        public float HitRadius { get; protected set; } = 1f;

        private readonly List<ISpell> pendingSpells = new();

        private Dictionary<Property, Dictionary</*spell4Id*/uint, ISpellPropertyModifier>> spellProperties = new();

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
                if (spell.IsFinished)
                    pendingSpells.Remove(spell);
            }
        }

        /// <summary>
        /// Add a <see cref="Property"/> modifier given a Spell4Id and <see cref="ISpellPropertyModifier"/> instance.
        /// </summary>
        public void AddSpellModifierProperty(ISpellPropertyModifier spellModifier, uint spell4Id)
        {
            if (spellProperties.TryGetValue(spellModifier.Property, out Dictionary<uint, ISpellPropertyModifier> spellDict))
            {
                if (spellDict.ContainsKey(spell4Id))
                    spellDict[spell4Id] = spellModifier;
                else
                    spellDict.Add(spell4Id, spellModifier);
            }
            else
            {
                spellProperties.Add(spellModifier.Property, new Dictionary<uint, ISpellPropertyModifier>
                {
                    { spell4Id, spellModifier }
                });
            }

            CalculateProperty(spellModifier.Property);
        }

        /// <summary>
        /// Remove a <see cref="Property"/> modifier by a Spell that is currently affecting this <see cref="IUnitEntity"/>.
        /// </summary>
        public void RemoveSpellProperty(Property property, uint spell4Id)
        {
            if (spellProperties.TryGetValue(property, out Dictionary<uint, ISpellPropertyModifier> spellDict))
                spellDict.Remove(spell4Id);

            CalculateProperty(property);
        }

        /// <summary>
        /// Remove all <see cref="Property"/> modifiers by a Spell that is currently affecting this <see cref="IUnitEntity"/>
        /// </summary>
        public void RemoveSpellProperties(uint spell4Id)
        {
            List<Property> propertiesWithSpell = spellProperties.Where(i => i.Value.ContainsKey(spell4Id)).Select(p => p.Key).ToList();

            foreach (Property property in propertiesWithSpell)
                RemoveSpellProperty(property, spell4Id);
        }

        /// <summary>
        /// Return all <see cref="IPropertyModifier"/> for this <see cref="IUnitEntity"/>'s <see cref="Property"/>
        /// </summary>
        private IEnumerable<ISpellPropertyModifier> GetSpellPropertyModifiers(Property property)
        {
            return spellProperties.ContainsKey(property) ? spellProperties[property].Values : Enumerable.Empty<ISpellPropertyModifier>();
        }

        protected override void CalculatePropertyValue(IPropertyValue propertyValue)
        {
            base.CalculatePropertyValue(propertyValue);

            // Run through spell adjustments first because they could adjust base properties
            // dataBits01 appears to be some form of Priority or Math Operator
            foreach (ISpellPropertyModifier spellModifier in GetSpellPropertyModifiers(propertyValue.Property)
                .OrderByDescending(s => s.Priority))
            {
                foreach (IPropertyModifier alteration in spellModifier.Alterations)
                {
                    // TODO: Add checks to ensure we're not modifying FlatValue and Percentage in the same effect?
                    switch (alteration.ModType)
                    {
                        case ModType.FlatValue:
                        case ModType.LevelScale:
                            propertyValue.Value += alteration.GetValue(Level);
                            break;
                        case ModType.Percentage:
                            propertyValue.Value *= alteration.GetValue();
                            break;
                    }
                }
            }
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

            // Cancel certain Spells / Buffs if required, when another ability is cast.
            // TODO: Improve this with certain rules, as there will be abilities that can be cast while stealthed, etc.
            if (parameters.UserInitiatedSpellCast)
            {
                if (this is IPlayer player)
                    player.Dismount();

                // TODO: This "effect" of removing Stealth when abilities are cast is handled by a Proc effect in the original spell. It'll trigger the removal of this buff when a player uses an ability. Once Procs are implemented, this can be removed.
                uint[] ignoredStealthBaseIds = new uint[]
                   {
                    30075,
                    23164,
                    30076
                   };
                if (Stealthed && !ignoredStealthBaseIds.Contains(parameters.SpellInfo.Entry.Spell4BaseIdBaseSpell))
                {
                    foreach ((uint castingId, List<EntityStatus> statuses) in StatusEffects)
                    {
                        if (statuses.Contains(EntityStatus.Stealth))
                        {
                            ISpell activeSpell = GetActiveSpell(i => i.CastingId == castingId);
                            activeSpell.Finish();
                        }
                    }
                }
            }

            var spell = new Spell.Spell(this, parameters);
            spell.Cast();
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
        /// Returns an active <see cref="ISpell"/> that is affecting this <see cref="IUnitEntity"/>
        /// </summary>
        public ISpell GetActiveSpell(Func<ISpell, bool> func)
        {
            return pendingSpells.FirstOrDefault(func);
        }
    }
}
