using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Abstract.Spell;
using NexusForever.GameTable;
using NexusForever.Script.Main.AI;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Event;
using NexusForever.Script.Template.Filter;
using NexusForever.Shared;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther.Script
{
    [ScriptFilterScriptName("RavenousReaperEntityScript")]
    public class RavenousReaperEntityScript : CombatAI
    {
        private enum Spell
        {
            CrushingFlurry = 46692,
            FocusedAssualt = 46790
        }

        #region Dependency Injection

        private readonly IScriptEventFactory eventFactory;
        private readonly IScriptEventManager eventManager;
        private readonly IGlobalQuestManager globalQuestManager;

        public RavenousReaperEntityScript(
            IFactory<ISpellParameters> spellParametersFactory,
            IGameTableManager gameTableManager,
            IScriptEventFactory eventFactory,
            IScriptEventManager eventManager,
            IGlobalQuestManager globalQuestManager)
            : base(spellParametersFactory, gameTableManager)
        {
            this.eventFactory = eventFactory;
            this.eventManager = eventManager;
            this.eventManager.OnScriptEvent += OnScriptEvent;
            this.globalQuestManager = globalQuestManager;
        }

        #endregion

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public override void OnLoad(ICreatureEntity owner)
        {
            base.OnLoad(owner);
            autoAttacks = [32920, 32921];
        }

        protected override void UpdateAI(double lastTick)
        {
            base.UpdateAI(lastTick);
            eventManager.Update(lastTick);
        }

        private void OnScriptEvent(IScriptEvent scriptEvent, uint? _)
        {
            switch (scriptEvent)
            {
                case IEntityCastEvent castEvent:
                    OnEntityCastEvent(castEvent);
                    break;
            }
        }

        private void OnEntityCastEvent(IEntityCastEvent @event)
        {
            switch ((Spell)@event.SpellId)
            {
                case Spell.CrushingFlurry:
                    eventManager.EnqueueEvent(TimeSpan.FromSeconds(10), @event);
                    break;
            }
        }

        /// <summary>
        /// Invoked when <see cref="IUnitEntity"/> enters combat.
        /// </summary>
        public override void OnEnterCombat()
        {
            IEntityCastEvent focusedAssaultEvent = eventFactory.CreateEvent<IEntityCastEvent>();
            focusedAssaultEvent.Initialise(entity, Spell.FocusedAssualt, false);
            eventManager.EnqueueEvent(TimeSpan.FromSeconds(4), focusedAssaultEvent);

            IEntityCastEvent crushingFlurryEvent = eventFactory.CreateEvent<IEntityCastEvent>();
            crushingFlurryEvent.Initialise(entity, Spell.CrushingFlurry, false);
            eventManager.EnqueueEvent(TimeSpan.FromSeconds(10), crushingFlurryEvent);
        }

        /// <summary>
        /// Invoked when <see cref="IUnitEntity"/> leaves combat.
        /// </summary>
        public override void OnLeaveCombat()
        {
            eventManager.CancelEvents();
        }

        /// <summary>
        /// Invoked when <see cref="IUnitEntity"/> is killed.
        /// </summary>
        public override void OnDeath()
        {
            if (entity.Map is not IMapInstance mapInstance)
                return;

            ICommunicatorMessage message = globalQuestManager.GetCommunicatorMessage(CommunicatorMessage.CaptainWeir6);
            foreach (IPlayer player in mapInstance.PlayerManager)
                message?.Send(player.Session);
        }
    }
}
