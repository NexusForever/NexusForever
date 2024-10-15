using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Script.Template.Event;
using NexusForever.Script.Template.Filter;
using NexusForever.Shared;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther.Script
{
    [ScriptFilterCreatureId(71221)]
    public class EthericPortalLargeEntityScript : EthericPortalEntityScript
    {
        #region Dependency Injection

        public EthericPortalLargeEntityScript(
            IScriptEventFactory eventFactory,
            IScriptEventManager eventManager,
            IFactory<IEntitySummonTemplate> templateFactory,
            IFactory<ISpellParameters> spellParameterFactory)
            : base(eventFactory, eventManager, templateFactory, spellParameterFactory)
        {
        }

        #endregion

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to <see cref="IBaseMap"/>.
        /// </summary>
        public override void OnAddToMap(IBaseMap map)
        {
            CreateTetheredOrganism(TimeSpan.FromSeconds(1.5), 60f.ToRadians());
            CreateTetheredOrganism(TimeSpan.FromSeconds(3.0), 30f.ToRadians());
            CreateTetheredOrganism(TimeSpan.FromSeconds(4.5), 0f);
            CreateTetheredOrganism(TimeSpan.FromSeconds(6.0), -30f.ToRadians());
            CreateTetheredOrganism(TimeSpan.FromSeconds(7.5), -60f.ToRadians());
        }
    }
}
