using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Script.Template.Event;
using NexusForever.Script.Template.Filter;
using NexusForever.Shared;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther.Script
{
    [ScriptFilterCreatureId(71132)]
    public class EthericPortalSmallEntityScript : EthericPortalEntityScript
    {
        #region Dependency Injection

        public EthericPortalSmallEntityScript(
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
            CreateTetheredOrganism(TimeSpan.FromSeconds(2), 45f.ToRadians());
            CreateTetheredOrganism(TimeSpan.FromSeconds(4), 0f);
            CreateTetheredOrganism(TimeSpan.FromSeconds(6), -45f.ToRadians());
        }
    }
}
