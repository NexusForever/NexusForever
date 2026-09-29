using System.Linq;
using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Tutorial
{
    /// <summary>
    /// Exile/Dominion "Navigating Nexus" (10527 / 10532).
    /// Despawns the starter-area barrier once the jump EnterArea is done so the hoverboard path opens.
    /// On achieve (combat projector activate), force-completes and teleports into the combat arena;
    /// <see cref="TutorialMapScript"/> grants Face of the Enemy on map add.
    /// Shared map: barrier removal is global for world 3460 (acceptable for NPE).
    /// </summary>
    [ScriptFilterOwnerId(10527, 10532)]
    public class NavigatingNexusQuestScript : IQuestScript, IOwnedScript<IQuest>
    {
        private const uint BarrierCreatureId = 73610;

        // Combat arena start — first path pad before the Battle Beast pack (entity 75094).
        // Midpoint (-125,290) sits inside rock geometry between path nodes; cinematic overlook
        // (-50.5,-861.4,307.8) is the intro camera, not the fight start.
        private static readonly Vector3 CombatArenaPosition = new(-134.6f, -875.6f, 285.5f);

        private IQuest owner;
        private bool barrierRemoved;
        private bool handedOff;

        public void OnLoad(IQuest owner)
        {
            this.owner = owner;

            // Character already past the jump gate (e.g. after !quest achieve / relog).
            if (owner.Any(o => o.Index == 0 && o.IsComplete()))
            {
                RemoveStarterBarrier(owner.Owner);
                barrierRemoved = true;
            }

            // Relog while Achieved (projector activated, hand-off never ran).
            if (owner.State == QuestState.Achieved)
                HandoffToCombat(owner.Owner);
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

        public void OnQuestStateChange(QuestState newState, QuestState oldState)
        {
            if (newState == QuestState.Achieved)
                HandoffToCombat(owner.Owner);
        }

        private void HandoffToCombat(IPlayer player)
        {
            if (handedOff || player == null)
                return;
            handedOff = true;

            // No quest receiver / communicator turn-in for this NPE quest.
            player.QuestManager.QuestForceComplete(owner.Id);

            player.Dismount();

            float x = CombatArenaPosition.X;
            float y = CombatArenaPosition.Y;
            float z = CombatArenaPosition.Z;
            // Snap to terrain when the map file has height data so we don't embed in props.
            float? terrainY = player.Map?.GetTerrainHeight(x, z);
            if (terrainY.HasValue)
                y = terrainY.Value;

            player.TeleportTo(3460, x, y, z);
            // Face of the Enemy is granted by TutorialMapScript after map add.
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
