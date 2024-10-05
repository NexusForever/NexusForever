using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Trigger;
using NexusForever.Game.Static.Event;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther
{
    [ScriptFilterOwnerId(50278)]
    public class GatherRingWorldLocationTriggerScript : IWorldLocationTriggerScript, IOwnedScript<IWorldLocationTriggerEntity>
    {
        private IWorldLocationTriggerEntity trigger;

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(IWorldLocationTriggerEntity owner)
        {
            trigger = owner;
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> enters the trigger.
        /// </summary>
        public void OnEntityEnter(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            trigger.Map.PublicEventManager.UpdateObjective(player, PublicEventObjectiveType.ParticipantsInTriggerVolume, 8242, 1);
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> leaves the trigger.
        /// </summary>
        public void OnEntityLeave(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            trigger.Map.PublicEventManager.UpdateObjective(player, PublicEventObjectiveType.ParticipantsInTriggerVolume, 8242, -1);
        }
    }
}
