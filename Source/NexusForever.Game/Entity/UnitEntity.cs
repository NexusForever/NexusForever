using NexusForever.Game.Abstract.Combat;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Combat;
using NexusForever.Game.Spell;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Event;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;
using NexusForever.Script;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Collection;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Entity
{
    public abstract class UnitEntity : WorldEntity, IUnitEntity
    {
        public float HitRadius { get; protected set; } = 1f;

        /// <summary>
        /// Guid of the <see cref="IUnitEntity"/> currently targeted.
        /// </summary>
        public uint? TargetGuid { get; private set; }

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

        /// <summary>
        /// Determines whether or not this <see cref="IUnitEntity"/> is in combat.
        /// </summary>
        public bool InCombat
        {
            get => inCombat;
            private set
            {
                if (inCombat == value)
                    return;

                inCombat = value;

                EnqueueToVisible(new ServerUnitEnteredCombat
                {
                    UnitId   = Guid,
                    InCombat = value
                }, true);
            }
        }

        private bool inCombat;

        public IThreatManager ThreatManager { get; private set; }

        /// <summary>
        /// Initial stab at a timer to regenerate Health & Shield values.
        /// </summary>
        private UpdateTimer statUpdateTimer = new UpdateTimer(0.25); // TODO: Long-term this should be absorbed into individual timers for each Stat regeneration method

        private readonly List<ISpell> pendingSpells = new();

        private Dictionary<Property, Dictionary</*spell4Id*/uint, ISpellPropertyModifier>> spellProperties = new();

        #region Dependency Injection

        public UnitEntity(IMovementManager movementManager,
            IEntitySummonFactory entitySummonFactory)
            : base(movementManager, entitySummonFactory)
        {
            ThreatManager = new ThreatManager(this);

            InitialiseHitRadius();
        }

        #endregion

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

            statUpdateTimer.Update(lastTick);
            if (statUpdateTimer.HasElapsed)
            {
                HandleStatUpdate(lastTick);
                statUpdateTimer.Reset();
            }
        }

        /// <summary>
        /// Initialise <see cref="IScriptCollection"/> for <see cref="IUnitEntity"/>.
        /// </summary>
        protected override void InitialiseScriptCollection()
        {
            scriptCollection = ScriptManager.Instance.InitialiseOwnedCollection<IUnitEntity>(this);
            ScriptManager.Instance.InitialiseEntityScripts<IUnitEntity>(scriptCollection, this);
        }

        /// <summary>
        /// Remove tracked <see cref="IGridEntity"/> that is no longer in vision range.
        /// </summary>
        public override void RemoveVisible(IGridEntity entity)
        {
            if (entity.Guid == TargetGuid)
                SetTarget((IWorldEntity)null);

            ThreatManager.RemoveHostile(entity.Guid);

            base.RemoveVisible(entity);
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
            return (this is IPlayer or INonPlayerEntity);
        }

        /// <summary>
        /// Deal damage to this <see cref="IUnitEntity"/> from the supplied <see cref="IUnitEntity"/>.
        /// </summary>
        public void TakeDamage(IUnitEntity attacker, IDamageDescription damageDescription)
        {
            if (!IsAlive || !attacker.IsAlive)
                return;

            // TODO: Calculate Threat properly
            ThreatManager.UpdateThreat(attacker, (int)damageDescription.RawDamage);

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
                OnDeath();
        }

        protected virtual void OnDeath()
        {
            DeathState = EntityDeathState.JustDied;

            foreach (ISpell spell in pendingSpells)
            {
                if (spell.IsCasting)
                    spell.CancelCast(CastResult.CasterCannotBeDead);
            }

            GenerateRewards();
            // TODO: schedule respawn

            ThreatManager.ClearThreatList();

            scriptCollection?.Invoke<IUnitScript>(s => s.OnDeath());

            deathState = EntityDeathState.Dead;
        }

        private void GenerateRewards()
        {
            foreach (uint targetGroupId in AssetManager.Instance.GetTargetGroupsForCreatureId(CreatureId))
                Map.PublicEventManager.UpdateObjective(PublicEventObjectiveType.KillTargetGroup, targetGroupId, 1);

            foreach (IHostileEntity hostile in ThreatManager)
            {
                IUnitEntity entity = GetVisible<IUnitEntity>(hostile.HatedUnitId);
                if (entity is IPlayer player)
                    RewardKiller(player);
            }
        }

        protected virtual void RewardKiller(IPlayer player)
        {
            player.QuestManager.ObjectiveUpdate(QuestObjectiveType.KillCreature, CreatureId, 1u);
            player.QuestManager.ObjectiveUpdate(QuestObjectiveType.KillCreature2, CreatureId, 1u);

            foreach (uint targetGroupId in AssetManager.Instance.GetTargetGroupsForCreatureId(CreatureId))
            {
                player.QuestManager.ObjectiveUpdate(QuestObjectiveType.KillTargetGroup, targetGroupId, 1u);
                player.QuestManager.ObjectiveUpdate(QuestObjectiveType.KillTargetGroups, targetGroupId, 1u);
            }

            // TODO: Reward XP
            // TODO: Reward Loot
            // TODO: Handle Achievements
        }

        /// <summary>
        /// Set target to supplied target guid.
        /// </summary>
        /// <remarks>
        /// A null target will clear the current target.
        /// </remarks>
        public void SetTarget(uint? target, uint threat = 0u)
        {
            SetTarget(target != null ? GetVisible<IWorldEntity>(target.Value) : null, threat);
        }

        /// <summary>
        /// Set target to supplied <see cref="IUnitEntity"/>.
        /// </summary>
        /// <remarks>
        /// A null target will clear the current target.
        /// </remarks>
        public virtual void SetTarget(IWorldEntity target, uint threat = 0u)
        {
            // notify current target they are no longer the target
            if (TargetGuid != null)
                GetVisible<IWorldEntity>(TargetGuid.Value)?.OnUntargeted(this);

            target?.OnTargeted(this);

            EnqueueToVisible(new ServerEntityTargetUnit
            {
                UnitId      = Guid,
                NewTargetId = target?.Guid ?? 0u,
                ThreatLevel = threat
            });

            TargetGuid = target?.Guid;
        }

        /// <summary>
        /// Invoked when a new <see cref="IHostileEntity"/> is added to the threat list.
        /// </summary>
        public virtual void OnThreatAddTarget(IHostileEntity hostile)
        {
            UpdateCombatState();
            scriptCollection?.Invoke<IUnitScript>(s => s.OnThreatAddTarget(hostile));
        }

        /// <summary>
        /// Invoked when an existing <see cref="IHostileEntity"/> is removed from the threat list.
        /// </summary>
        public virtual void OnThreatRemoveTarget(IHostileEntity hostile)
        {
            UpdateCombatState();
            scriptCollection?.Invoke<IUnitScript>(s => s.OnThreatRemoveTarget(hostile));
        }

        /// <summary>
        /// Invoked when an existing <see cref="IHostileEntity"/> is update on the threat list.
        /// </summary>
        public virtual void OnThreatChange(IHostileEntity hostile)
        {
            scriptCollection?.Invoke<IUnitScript>(s => s.OnThreatChange(hostile));
        }

        private void UpdateCombatState()
        {
            // ensure conditions for combat state change are met
            if (ThreatManager.IsThreatened == InCombat)
                return;

            InCombat   = ThreatManager.IsThreatened;
            Sheathed   = !inCombat;
            StandState = inCombat ? StandState.Stand : StandState.State0;
        }
    }
}
