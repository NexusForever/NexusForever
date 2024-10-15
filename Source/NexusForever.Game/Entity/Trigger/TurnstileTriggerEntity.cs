using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Trigger;
using NexusForever.Script;

namespace NexusForever.Game.Entity.Trigger
{
    public class TurnstileTriggerEntity : GridTriggerEntity, ITurnstileGridTriggerEntity
    {
        private uint objectId;

        #region Dependency Injection

        public TurnstileTriggerEntity(
            IScriptManager scriptManager)
            : base(scriptManager)
        {
        }

        #endregion

        /// <summary>
        /// Initialise turnstile trigger with supplied id, range and objective object id.
        /// </summary>
        public void Initialise(uint id, float range, uint objectId)
        {
            Initialise(id, range);
            this.objectId = objectId;
        }

        protected override void AddToRange(IGridEntity entity)
        {
            base.AddToRange(entity);

            if (entity is not IPlayer)
                return;

            Map.PublicEventManager.UpdateObjective(Static.Event.PublicEventObjectiveType.Turnstile, objectId, 1);
        }
    }
}
