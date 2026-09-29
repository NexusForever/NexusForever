using System.Linq;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Tutorial
{
    /// <summary>
    /// Exile/Dominion "Navigating Nexus" (10527 / 10532).
    /// Despawns the starter-area barrier once the jump EnterArea is done so the hoverboard path opens.
    /// Shared map: barrier removal is global for world 3460 (acceptable for NPE).
    /// </summary>
    [ScriptFilterOwnerId(10527, 10532)]
    public class NavigatingNexusQuestScript : IQuestScript, IOwnedScript<IQuest>
    {
        private const uint BarrierCreatureId = 73610;

        private IQuest owner;
        private bool barrierRemoved;

        public void OnLoad(IQuest owner)
        {
            this.owner = owner;

            // Character already past the jump gate (e.g. after !quest achieve / relog).
            if (owner.Any(o => o.Index == 0 && o.IsComplete()))
            {
                RemoveStarterBarrier(owner.Owner);
                barrierRemoved = true;
            }
        }

        public void OnObjectiveUpdate(IQuestObjective objective)
        {
            if (barrierRemoved || !objective.IsComplete())
                return;

            // Obj0 = jump EnterArea; obj2 = hoverboard projector SucceedCSI (index may vary if optional skipped)
            if (objective.Index is not (0 or 2))
                return;

            RemoveStarterBarrier(owner.Owner);
            barrierRemoved = true;
        }

        private static void RemoveStarterBarrier(IPlayer player)
        {
            if (player?.Map == null)
                return;

            foreach (IWorldEntity barrier in player.GetVisibleCreature<IWorldEntity>(BarrierCreatureId).ToList())
                barrier.RemoveFromMap();
        }
    }
}
