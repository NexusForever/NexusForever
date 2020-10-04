using NexusForever.Game.Abstract.Combat;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Combat;
using NexusForever.Game.Spell;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Entity
{
    public abstract class UnitEntity : WorldEntity, IUnitEntity
    {
        public float HitRadius { get; protected set; } = 1f;

        /// <summary>
        /// Determines whether or not this <see cref="IUnitEntity"/> is alive.
        /// </summary>
        public bool IsAlive => Health > 0u && deathState == null;

        protected EntityDeathState? DeathState
        {
            get => deathState;
            set
            {
                deathState = value;

                if (deathState is null or EntityDeathState.JustDied)
                {
                    EnqueueToVisible(new ServerEntityDeathState
                    {
                        UnitId    = Guid,
                        Dead      = !IsAlive,
                        Reason    = 0, // client does nothing with this value
                        RezHealth = IsAlive ? Health : 0u
                    }, true);
                }
            }
        }

        private EntityDeathState? deathState;

        public bool InCombat
        {
            get => inCombat;
            private set
            {
                if (inCombat == value)
                    return;

                inCombat = value;
                OnCombatStateChange(value);

                EnqueueToVisible(new ServerUnitEnteredCombat
                {
                    UnitId = Guid,
                    InCombat = value
                }, true);
            }
        }

        private bool inCombat;

        public IThreatManager ThreatManager { get; private set; }

        protected uint currentTargetUnitId;

        /// <summary>
        /// Initial stab at a timer to regenerate Health & Shield values.
        /// </summary>
        private UpdateTimer statUpdateTimer = new UpdateTimer(0.25); // TODO: Long-term this should be absorbed into individual timers for each Stat regeneration method

        private readonly List<ISpell> pendingSpells = new();

        private Dictionary<Property, Dictionary</*spell4Id*/uint, ISpellPropertyModifier>> spellProperties = new();

        protected UnitEntity(EntityType type)
            : base(type)
        {
            ThreatManager = new ThreatManager(this);

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

            ThreatManager.Update(lastTick);

            statUpdateTimer.Update(lastTick);
            if (statUpdateTimer.HasElapsed)
            {
                HandleStatUpdate(lastTick);
                statUpdateTimer.Reset();
            }
        }

        /// <summary>
        /// Invoked when <see cref="IUnitEntity"/> is removed from <see cref="IBaseMap"/>.
        /// </summary>
        public override void OnRemoveFromMap()
        {
            // TODO: Delay OnRemoveFromMap from firing immediately on DC. Allow players to die between getting disconnected and being removed from map :D
            ThreatManager.ClearThreatList();

            base.OnRemoveFromMap();
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
        /// Handles regeneration of Stat Values. Used to provide a hook into the Update method, for future implementation.
        /// </summary>
        private void HandleStatUpdate(double lastTick)
        {
            if (!IsAlive)
                return;

            // TODO: This should probably get moved to a Calculation Library/Manager at some point. There will be different timers on Stat refreshes, but right now the timer is hardcoded to every 0.25s.
            // Probably worth considering an Attribute-grouped Class that allows us to run differentt regeneration methods & calculations for each stat.

            if (Health < MaxHealth)
                ModifyHealth((uint)(MaxHealth / 200f), DamageType.Heal, null);

            if (Shield < MaxShieldCapacity)
                Shield += (uint)(MaxShieldCapacity * GetPropertyValue(Property.ShieldRegenPct) * statUpdateTimer.Duration);
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
            if (!IsAlive)
                return;

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

        /// <summary>
        /// Determine if this <see cref="IUnitEntity"/> can attack supplied <see cref="IUnitEntity"/>.
        /// </summary>
        public virtual bool CanAttack(IUnitEntity target)
        {
            if (!IsAlive)
                return false;

            if (!target.IsValidAttackTarget() || !IsValidAttackTarget())
                return false;

            return GetDispositionTo(target.Faction1) < Disposition.Friendly;
        }

        /// <summary>
        /// Returns whether or not this <see cref="IUnitEntity"/> is an attackable target.
        /// </summary>
        public bool IsValidAttackTarget()
        {
            // TODO: Expand on this. There's bound to be flags or states that should prevent an entity from being attacked.
            return (this is IPlayer or INonPlayer);
        }

        /// <summary>
        /// Deal damage to this <see cref="IUnitEntity"/> from the supplied <see cref="IUnitEntity"/>.
        /// </summary>
        public void TakeDamage(IUnitEntity attacker, IDamageDescription damageDescription)
        {
            if (!IsAlive || !attacker.IsAlive)
                return;

            // TODO: Add Threat

            Shield -= damageDescription.ShieldAbsorbAmount;
            ModifyHealth(damageDescription.AdjustedDamage, damageDescription.DamageType, attacker);
        }

        /// <summary>
        /// Modify the health of this <see cref="IUnitEntity"/> by the supplied amount.
        /// </summary>
        /// <remarks>
        /// If the <see cref="DamageType"/> is <see cref="DamageType.Heal"/> amount is added to current health otherwise subtracted.
        /// </remarks>
        public virtual void ModifyHealth(uint amount, DamageType type, IUnitEntity source)
        {
            long newHealth = Health;
            if (type == DamageType.Heal)
                newHealth += amount;
            else
                newHealth -= amount;

            Health = (uint)Math.Clamp(newHealth, 0u, MaxHealth);

            if (Health == 0)
                OnDeath(source);
        }

        protected virtual void OnDeath(IUnitEntity killer)
        {
            DeathState = EntityDeathState.JustDied;

            foreach (ISpell spell in pendingSpells)
            {
                if (spell.IsCasting)
                    spell.CancelCast(CastResult.CasterCannotBeDead);
            }

            GenerateRewards(killer);
            // TODO: schedule respawn

            deathState = EntityDeathState.Dead;
        }

        private void GenerateRewards(IUnitEntity killer)
        {
            // TODO: replace with threat targets
            if (killer is IPlayer player)
                RewardKiller(player);
        }

        protected virtual void RewardKiller(IPlayer player)
        {
            player.QuestManager.ObjectiveUpdate(QuestObjectiveType.KillCreature, CreatureId, 1u);
            player.QuestManager.ObjectiveUpdate(QuestObjectiveType.KillCreature2, CreatureId, 1u);
            player.QuestManager.ObjectiveUpdate(QuestObjectiveType.KillTargetGroup, CreatureId, 1u);
            player.QuestManager.ObjectiveUpdate(QuestObjectiveType.KillTargetGroups, CreatureId, 1u);

            // TODO: Reward XP
            // TODO: Reward Loot
            // TODO: Handle Achievements
        }

        private void CheckCombatStateChange(IEnumerable<IHostileEntity> hostiles = null)
        {
            if (!IsValidAttackTarget())
                return;

            // TODO: Add other checks as necessary
            hostiles ??= ThreatManager.GetThreatList();

            if (hostiles.Count() > 0)
                InCombat = true;
            else
                InCombat = false;

            SelectTarget();
        }

        /// <summary>
        /// Invoked when this <see cref="IUnitEntity"/> is asked to select a target for an attack.
        /// </summary>
        public virtual void SelectTarget(IEnumerable<IHostileEntity> hostiles = null)
        {
            // deliberately empty
        }

        protected void SetTarget(uint targetUnitId, uint threatLevel = 0u)
        {
            if (currentTargetUnitId == targetUnitId)
                return;

            currentTargetUnitId = targetUnitId;
            EnqueueToVisible(new ServerEntityTargetUnit
            {
                UnitId = Guid,
                NewTargetId = targetUnitId,
                ThreatLevel = threatLevel
            });

            if (currentTargetUnitId != 0u)
                EnqueueToVisible(new ServerEntityAggroSwitch
                {
                    UnitId = Guid,
                    TargetId = currentTargetUnitId
                });
        }

        /// <summary>
        /// Invoked when <see cref="IThreatManager"/> adds a <see cref="IHostileEntity"/>.
        /// </summary>
        public virtual void OnThreatAddTarget(IHostileEntity hostile)
        {
            if (this is IPlayer)
                return;

            if (currentTargetUnitId == 0u)
                SetTarget(hostile.HatedUnitId, hostile.Threat);
        }

        /// <summary>
        /// Invoked when <see cref="IThreatManager"/> removes a <see cref="IHostileEntity"/>.
        /// </summary>
        public virtual void OnThreatRemoveTarget(IHostileEntity hostile)
        {
            SelectTarget();
        }

        /// <summary>
        /// Invoked when <see cref="IThreatManager"/> updates a <see cref="IHostileEntity"/>.
        /// </summary>
        /// <param name="hostiles"></param>
        public virtual void OnThreatChange(IEnumerable<IHostileEntity> hostiles)
        {
            CheckCombatStateChange(hostiles);
            SelectTarget();
        }

        /// <summary>
        /// Invoked when this <see cref="IUnitEntity"/> combat state is changed.
        /// </summary>
        public virtual void OnCombatStateChange(bool inCombat)
        {
            Sheathed = !inCombat;

            if (inCombat)
                StandState = StandState.Stand;
            else
                StandState = StandState.State0;
        }
    }
}
