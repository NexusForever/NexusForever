using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Trigger;
using NexusForever.Game.Static.Event;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther.Script
{
    [ScriptFilterOwnerId(8242)]
    public class UpperDeckTeleportGridTriggerEntityScript : IGridEntityScript, IOwnedScript<IGridTriggerEntity>
    {
        protected IGridTriggerEntity trigger;

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(IGridTriggerEntity owner)
        {
            trigger = owner;
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to range check range.
        /// </summary>
        public void OnEnterRange(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            player.TeleportToLocal(new Vector3(-53.353714f, -841.44684f, 164.51099f), false);
            trigger.Map.PublicEventManager.UpdateObjective(PublicEventObjectiveType.Script, 8242, 1);
        }
    }
}
