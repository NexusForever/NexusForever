using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Tutorial
{
    [ScriptFilterCreatureId(73498u, 74745u, 74746u)]
    public sealed class TutorialCircuitSwitchScript : IWorldEntityScript, IOwnedScript<ISimpleEntity>
    {
        private const uint TutorialWorld = 3460u;
        private const uint CircuitZone = 5968u;
        private const uint ActivationSpell = 1817u;
        private const float ActivationRange = 5f;

        private readonly IGameTableManager gameTables;
        private ISimpleEntity circuitSwitch;

        public TutorialCircuitSwitchScript(IGameTableManager gameTables) => this.gameTables = gameTables;

        public void OnLoad(ISimpleEntity owner) => circuitSwitch = owner;

        public void OnActivate(IPlayer player) => OnActivateCast(player);

        public void OnActivateCast(IPlayer player)
        {
            if (circuitSwitch is not { EntityId: > 0u, CreatureEntry.Spell4IdActivate00: ActivationSpell,
                    Map: { Entry.Id: TutorialWorld } map } ||
                player is not { IsAlive: true, IsLoading: false, Zone.Id: CircuitZone } ||
                player.Faction1 != player.Faction2 || !ReferenceEquals(player.Map, map) ||
                !ReferenceEquals(player.GetVisible<IWorldEntity>(circuitSwitch.Guid), circuitSwitch) ||
                !(Vector3.DistanceSquared(player.Position, circuitSwitch.Position) <= ActivationRange * ActivationRange))
                return;

            (byte slot, uint markerId, uint exileGoal, uint dominionGoal) = circuitSwitch.CreatureId switch
            {
                73498u => ((byte)0, 52742u, 21293u, 21306u),
                74745u => ((byte)1, 52743u, 21345u, 21359u),
                74746u => ((byte)2, 52744u, 21346u, 21360u),
                _ => default
            };
            (ushort questId, uint goalId) = player.Faction1 switch
            {
                Faction.Exile => ((ushort)10519, exileGoal),
                Faction.Dominion => ((ushort)10522, dominionGoal),
                _ => default
            };
            if (questId == 0 || markerId == 0 ||
                gameTables.WorldLocation2.GetEntry(markerId) is not { WorldId: TutorialWorld, WorldZoneId: CircuitZone } marker ||
                !(Vector3.DistanceSquared(circuitSwitch.Position, Position(marker)) <= 1f))
                return;

            // These switches use an ordinary Activate spell even though their objectives are typed SucceedCSI.
            if (gameTables.Spell4.GetEntry(ActivationSpell) is not { } activation ||
                gameTables.Spell4Base.GetEntry(activation.Spell4BaseIdBaseSpell) is not { CastMethod: not 5u, ClientSideInteractionId: 0u })
                return;

            IQuest quest = player.QuestManager.GetActiveQuests().FirstOrDefault(active =>
                active.Id == questId && active.State == QuestState.Accepted && !active.PendingDelete);
            IQuestObjective goal = quest?.FirstOrDefault(objective => objective.Index == slot);
            if (goal?.ObjectiveInfo is not { Type: QuestObjectiveType.SucceedCSI, Entry.Count: 1u } info ||
                info.Id != goalId || info.Entry.Data != circuitSwitch.CreatureId ||
                info.Entry.WorldLocationsIdIndicator00 != markerId ||
                quest.Any(objective => objective.Index < slot && !objective.IsComplete()))
                return;

            quest.ObjectiveUpdate(goalId, 1u);
        }

        private static Vector3 Position(WorldLocation2Entry marker) => new(marker.Position0, marker.Position1, marker.Position2);
    }
}
