using NexusForever.Game.Abstract.Entity;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther
{
    [ScriptFilterCreatureId(71266)]
    [ScriptFilterActivePropId(7059788, 7024518)]
    public class PrimaryPowerPlantDoorEntityScript : IWorldEntityScript, IOwnedScript<IDoorEntity>
    {
        private IDoorEntity door;

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(IDoorEntity owner)
        {
            door = owner;
            door.SetInRangeCheck(15f);
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to range check range.
        /// </summary>
        public void OnEnterRange(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            door.Map.PublicEventManager.UpdateObjective(player, Game.Static.Event.PublicEventObjectiveType.ParticipantsInTriggerVolume, 8283, 1);
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is removed from range check range.
        /// </summary>
        public void OnExitRange(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            door.Map.PublicEventManager.UpdateObjective(player, Game.Static.Event.PublicEventObjectiveType.ParticipantsInTriggerVolume, 8283, -1);
        }
    }
}
