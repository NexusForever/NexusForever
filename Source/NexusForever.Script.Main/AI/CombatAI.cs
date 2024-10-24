using NexusForever.Game.Abstract.Combat;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;
using NexusForever.Game.Abstract.Entity.Movement.Generator;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Entity.Movement.Spline;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;
using NexusForever.GameTable;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;
using NexusForever.Shared.Game;
using NexusForever.Shared;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Script.Main.AI
{
    //[ScriptFilterIgnore]
    public class CombatAI : IUnitScript, IOwnedScript<ICreatureEntity>
    {
        protected ICreatureEntity entity;

        private int autoAttackIndex = 0;
        protected List<uint> autoAttacks = [5649, 5652];
        private readonly UpdateTimer autoAttackTimer = new(TimeSpan.FromSeconds(1.5d));

        private float chaseDistance = 5f;
        private readonly UpdateTimer chaseDistanceTimer = new(TimeSpan.FromSeconds(1f));

        #region Dependency Injection

        private readonly IFactory<ISpellParameters> spellParametersFactory;
        private readonly IGameTableManager gameTableManager;

        public CombatAI(
            IFactory<ISpellParameters> spellParametersFactory,
            IGameTableManager gameTableManager)
        {
            this.spellParametersFactory = spellParametersFactory;
            this.gameTableManager = gameTableManager;
        }

        #endregion

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public virtual void OnLoad(ICreatureEntity owner)
        {
            entity = owner;
            entity.SetInRangeCheck(15f);
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            if (!entity.IsAlive)
                return;

            if (!entity.TargetGuid.HasValue)
                return;

            if (entity.IsCasting())
                return;

            autoAttackTimer.Update(lastTick);
            if (autoAttackTimer.HasElapsed)
            {
                DoAutoAttack();
                autoAttackTimer.Reset();
            }

            chaseDistanceTimer.Update(lastTick);
            if (chaseDistanceTimer.HasElapsed)
            {
                DoChase();
                chaseDistanceTimer.Reset();
            }
        }

        private void DoAutoAttack()
        {
            if (autoAttacks.Count == 0)
                return;

            if (!entity.TargetGuid.HasValue)
                return;

            IUnitEntity target = entity.Map.GetEntity<IUnitEntity>(entity.TargetGuid.Value);
            if (target == null)
                return;

            uint spell4Id = autoAttacks[autoAttackIndex];
            autoAttackIndex = (autoAttackIndex++) % autoAttacks.Count;

            Spell4Entry spell4Entry = gameTableManager.Spell4.GetEntry(spell4Id);
            if (spell4Entry == null)
                return;

            chaseDistance = Math.Min(chaseDistance, spell4Entry.TargetMaxRange);

            if (entity.Position.GetDistance(target.Position) > spell4Entry.TargetMaxRange)
                return;

            ISpellParameters spellParameters = spellParametersFactory.Resolve();
            spellParameters.PrimaryTargetId = entity.TargetGuid.Value;
            entity.CastSpell(spell4Id, spellParameters);
        }

        private void DoChase()
        {
            if (!entity.TargetGuid.HasValue)
                return;

            IUnitEntity target = entity.Map.GetEntity<IUnitEntity>(entity.TargetGuid.Value);
            if (target == null)
                return;

            if (entity.Position.GetDistance(target.Position) < chaseDistance)
                return;
            
            entity.MovementManager.Chase(target, chaseDistance / 2);
        }

        /// <summary>
        /// Invoked when <see cref="IPositionCommand"/> is finalised.
        /// </summary>
        public void OnPositionEntityCommandFinalise(IPositionCommand command)
        {
            if (!entity.TargetGuid.HasValue)
                return;

            entity.MovementManager.SetRotationFaceUnit(entity.TargetGuid.Value);
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to range check range.
        /// </summary>
        public void OnEnterRange(IGridEntity entity)
        {
            AggroEntity(entity);
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is removed from vision range.
        /// </summary>
        public void OnRemoveVisibleEntity(IGridEntity entity)
        {
            if (entity.Guid == this.entity.TargetGuid)
                SelectTarget();
        }

        /// <summary>
        /// Invoked when health is changed by source <see cref="IUnitEntity"/>.
        /// </summary>
        public void OnHealthChange(IUnitEntity source, uint amount, DamageType type)
        {
            if (type == DamageType.Heal)
                return;

            AggroEntity(source);
        }

        private void AggroEntity(IGridEntity source)
        {
            if (!entity.IsAlive)
                return;

            if (entity.InCombat)
                return;

            if (source is not IPlayer player)
                return;

            if (entity.GetDispositionTo(player.Faction1) != Disposition.Hostile)
                return;

            var spellParameters = spellParametersFactory.Resolve();
            entity.CastSpell(41368, spellParameters);

            entity.MovementManager.Finalise();
            entity.MovementManager.SetRotationFaceUnit(player.Guid);

            entity.ThreatManager.UpdateThreat(player, 1);
        }

        /// <summary>
        /// Invoked when a new <see cref="IHostileEntity"/> is added to the threat list.
        /// </summary>
        public void OnThreatAddTarget(IHostileEntity hostile)
        {
            SelectTarget();
        }

        /// <summary>
        /// Invoked when an existing <see cref="IHostileEntity"/> is removed from the threat list.
        /// </summary>
        public void OnThreatRemoveTarget(IHostileEntity hostile)
        {
            SelectTarget();
        }

        /// <summary>
        /// Invoked when an existing <see cref="IHostileEntity"/> is update on the threat list.
        /// </summary>
        public void OnThreatChange(IHostileEntity hostile)
        {
            SelectTarget();
        }

        protected virtual void SelectTarget()
        {
            IHostileEntity hostile = entity.ThreatManager.GetTopHostile();
            if (hostile == null)
            {
                Reset();
                return;
            }

            if (entity.TargetGuid == hostile.HatedUnitId)
                return;

            entity.SetTarget(hostile.HatedUnitId, hostile.Threat);
        }

        private void Reset()
        {
            entity.SetTarget((IWorldEntity)null);

            entity.ModifyHealth(entity.MaxHealth, DamageType.Heal, null);

            float speed = entity.GetPropertyValue(Property.MoveSpeedMultiplier) * 10f;
            entity.MovementManager.LaunchSpline([entity.Position, entity.LeashPosition], SplineType.Linear, SplineMode.OneShot, speed);
        }
    }
}
