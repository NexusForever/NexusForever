using NexusForever.Game.Abstract.Entity;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther.Script
{
    [ScriptFilterCreatureId(71266)]
    [ScriptFilterActivePropId(7024519)]
    public class CrewQuatersDoorEntityScript : AutomaticDoorEntityScript
    {
        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to range check range.
        /// </summary>
        public override void OnEnterRange(IGridEntity entity)
        {
            base.OnEnterRange(entity);

            if (entity is not IPlayer)
                return;

            door.Map.PublicEventManager.UpdateObjective(PublicEventObjective.EnterCrewQuarters, 1);
        }
    }
}
