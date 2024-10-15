using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Event;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Spell;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Event;
using NexusForever.Shared;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther.Script
{
    public abstract class EthericPortalEntityScript : INonPlayerScript, IOwnedScript<INonPlayerEntity>
    {
        private INonPlayerEntity entity;

        private uint portalCount;

        #region Dependency Injection

        private readonly IScriptEventFactory eventFactory;
        private readonly IScriptEventManager eventManager;
        private readonly IFactory<IEntitySummonTemplate> templateFactory;
        private readonly IFactory<ISpellParameters> spellParameterFactory;

        public EthericPortalEntityScript(
            IScriptEventFactory eventFactory,
            IScriptEventManager eventManager,
            IFactory<IEntitySummonTemplate> templateFactory,
            IFactory<ISpellParameters> spellParameterFactory)
        {
            this.eventFactory = eventFactory;
            this.eventManager = eventManager;
            this.templateFactory = templateFactory;
            this.spellParameterFactory = spellParameterFactory;
        }

        #endregion

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(INonPlayerEntity owner)
        {
            entity = owner;

            entity.SummonFactory.OnSummon += OnSummon;
            entity.SummonFactory.OnUnsummon += OnUnsummon;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            eventManager.Update(lastTick);
        }

        protected void CreateTetheredOrganism(TimeSpan time, float angle)
        {
            IEntitySummonTemplate template = templateFactory.Resolve();
            template.CreatureId    = 71133;
            template.DisplayInfoId = 28700;
            template.Faction       = (Faction)218;

            float entityAngle = -entity.Rotation.X;
            entityAngle -= MathF.PI / 2;
            Vector3 position = entity.Position.GetPoint2D(entityAngle + angle, 5f);

            var @event = eventFactory.CreateEvent<IEntitySummonEvent<INonPlayerEntity>>();
            @event.Initialise(entity.SummonFactory, template, position, entity.Rotation);
            eventManager.EnqueueEvent(time, @event);
        }

        private void OnSummon(uint guid)
        {
            portalCount++;
        }

        private void OnUnsummon(uint guid)
        {
            portalCount--;

            // TODO: implement, this spell should shrink and damage the portal
            //ISpellParameters parameters = spellParameterFactory.Resolve();
            //entity.CastSpell(81642, parameters);

            if (portalCount == 0)
                entity.ModifyHealth(entity.Health, DamageType.Magic, null);
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to <see cref="IBaseMap"/>.
        /// </summary>
        public abstract void OnAddToMap(IBaseMap map);

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is remove from <see cref="IBaseMap"/>.
        /// </summary>
        public void OnRemoveFromMap(IBaseMap map)
        {
            eventManager.CancelEvents();
        }

        /// <summary>
        /// Invoked when <see cref="IUnitEntity"/> is killed.
        /// </summary>
        public void OnDeath()
        {
            entity.Map.PublicEventManager.UpdateObjective(PublicEventObjectiveType.Script, 0, 1);
            entity.RemoveFromMap();
        }
    }
}
