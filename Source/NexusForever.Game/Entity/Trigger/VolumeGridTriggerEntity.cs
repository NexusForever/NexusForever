using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Trigger;
using NexusForever.Game.Static.Event;
using NexusForever.Script;

namespace NexusForever.Game.Entity.Trigger
{
    public class VolumeGridTriggerEntity : GridTriggerEntity, IVolumeGridTriggerEntity
    {
        private uint objectId;

        #region Dependency Injection

        public VolumeGridTriggerEntity(
            IScriptManager scriptManager)
            : base(scriptManager)
        {
        }

        #endregion

        /// <summary>
        /// Initialise volume trigger with supplied id, range and objective object id.
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

            Map.PublicEventManager.UpdateObjective(PublicEventObjectiveType.ParticipantsInTriggerVolume, objectId, 1);
        }

        protected override void RemoveFromRange(IGridEntity entity)
        {
            if (entity is not IPlayer)
                return;

            Map.PublicEventManager.UpdateObjective(PublicEventObjectiveType.ParticipantsInTriggerVolume, objectId, -1);

            base.RemoveFromRange(entity);
        }
    }
}
