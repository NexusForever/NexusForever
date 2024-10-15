using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;
using NexusForever.Game.Static.Entity.Movement.Spline;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;
using NexusForever.Shared;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther.Script
{
    [ScriptFilterCreatureId(71832)]
    public class KatjaZarkhovFloatingEntityScript : INonPlayerScript, IOwnedScript<INonPlayerEntity>
    {
        private INonPlayerEntity entity;

        #region Dependency Injection

        private readonly IFactory<ISpellParameters> spellParameterFactory;

        public KatjaZarkhovFloatingEntityScript(
            IFactory<ISpellParameters> spellParameterFactory)
        {
            this.spellParameterFactory = spellParameterFactory;
        }

        #endregion

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(INonPlayerEntity owner)
        {
            entity = owner;

            entity.MovementManager.SetMode(ModeType.Swim);
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to <see cref="IBaseMap"/>.
        /// </summary>
        public void OnAddToMap(IBaseMap map)
        {
            var portalEntity = entity.GetVisibleCreature<INonPlayerEntity>(71134).SingleOrDefault();
            if (portalEntity == null)
                return;

            ISpellParameters parameters = spellParameterFactory.Resolve();
            parameters.PrimaryTargetId = portalEntity.Guid;

            entity.CastSpell(81640, parameters);
        }

        /// <summary>
        /// Invoked when <see cref="IPositionCommand"/> is finalised.
        /// </summary>
        public void OnPositionEntityCommandFinalise(IPositionCommand command)
        {
            if (command.Command != EntityCommand.SetPositionProjectile)
                return;

            entity.RemoveFromMap();
        }

        public void StartMoveToPortal()
        {
            entity.MovementManager.LaunchSpline(22523, SplineMode.OneShot, 0.2f, false);
        }

        public void KnockbackToFloor()
        {
            // TODO: this should be handled by spell 83100
            entity.MovementManager.SetPositionProjectile(1000, 40f, new Vector3(-53.6292f, -836.1417f, 362.947f));
        }
    }
}
