using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;
using NexusForever.Shared;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther.Script
{
    [ScriptFilterCreatureId(71133)]
    public class TetheredOrganismEntityScript : INonPlayerScript, IOwnedScript<INonPlayerEntity>
    {
        private INonPlayerEntity entity;

        #region Dependency Injection

        private readonly IFactory<ISpellParameters> spellParameterFactory;

        public TetheredOrganismEntityScript(
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
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to <see cref="IBaseMap"/>.
        /// </summary>
        public void OnAddToMap(IBaseMap map)
        {
            if (!entity.SummonerGuid.HasValue)
                return;

            var portalEntity = map.GetEntity<INonPlayerEntity>(entity.SummonerGuid.Value);
            if (portalEntity == null)
                return;

            ISpellParameters parameters = spellParameterFactory.Resolve();
            parameters.PrimaryTargetId = portalEntity.Guid;

            entity.CastSpell(81640, parameters);
        }

        /// <summary>
        /// Invoked when <see cref="IUnitEntity"/> is killed.
        /// </summary>
        public void OnDeath()
        {
            entity.RemoveFromMap();
        }
    }
}
