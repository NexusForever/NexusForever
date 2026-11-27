using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;
using NexusForever.Shared.Game;

namespace NexusForever.Script.Main.Tutorial
{
    [ScriptFilterCreatureId(Phineas)]
    public sealed class TutorialHousingGuideScript : IWorldEntityScript, IOwnedScript<ICreatureEntity>
    {
        private const uint TutorialWorld = 3460u;
        private const uint HousingZone = 5969u;
        private const uint Phineas = 73421u;
        private const uint GuideLocation = 52819u;
        private const uint GuideArea = 8581u;
        private const uint FindGuideIndex = 5u;
        private const float PlacementTolerance = 1f;
        private const float DiscoveryRange = 5f;

        private readonly IGameTableManager gameTableManager;
        private readonly UpdateTimer discoveryTimer = new(.5d);
        private ICreatureEntity guide;

        public TutorialHousingGuideScript(IGameTableManager gameTableManager) =>
            this.gameTableManager = gameTableManager;

        public void OnLoad(ICreatureEntity owner) => guide = owner;

        public void Update(double lastTick)
        {
            discoveryTimer.Update(lastTick);
            if (!discoveryTimer.HasElapsed)
                return;

            discoveryTimer.Reset();
            if (guide is not { EntityId: > 0u, CreatureId: Phineas, IsAlive: true,
                    Map: { Entry.Id: TutorialWorld } map } ||
                gameTableManager.WorldLocation2.GetEntry(GuideLocation) is not
                    { WorldId: TutorialWorld, WorldZoneId: HousingZone } location ||
                !(Vector3.DistanceSquared(guide.Position, new Vector3(location.Position0, location.Position1,
                    location.Position2)) <= PlacementTolerance * PlacementTolerance))
                return;

            // Sample visibility as players finish loading or clear the preceding goals near the guide.
            // An enter-range callback alone would require them to leave and re-enter.
            foreach (IPlayer player in guide.GetVisible().OfType<IPlayer>().ToArray())
            {
                if (player is not { IsAlive: true, IsLoading: false, Zone.Id: HousingZone } ||
                    !ReferenceEquals(player.Map, map) || player.Faction1 != player.Faction2 ||
                    !(Vector3.DistanceSquared(player.Position, guide.Position) <= DiscoveryRange * DiscoveryRange) ||
                    !ReferenceEquals(player.GetVisible<IWorldEntity>(guide.Guid), guide))
                    continue;

                CreditDiscovery(player);
            }
        }

        private static void CreditDiscovery(IPlayer player)
        {
            (ushort QuestId, uint ObjectiveId) step = player.Faction1 switch
            {
                Faction.Exile => ((ushort)10525, 21382u),
                Faction.Dominion => ((ushort)10526, 21383u),
                _ => default
            };
            if (step.QuestId == 0u)
                return;

            IQuest quest = player.QuestManager.GetActiveQuests().FirstOrDefault(active =>
                active.Id == step.QuestId && active is { State: QuestState.Accepted, PendingDelete: false } &&
                active.Info.Entry.WorldLocation2IdReceiver == GuideLocation);
            IQuestObjective objective = quest?.FirstOrDefault(goal => goal.Index == FindGuideIndex);
            if (objective == null || objective.IsComplete() || objective.ObjectiveInfo is not { Type: QuestObjectiveType.EnterArea, Entry:
                    { Data: GuideArea, Count: 1u, WorldLocationsIdIndicator00: GuideLocation } } info ||
                info.Id != step.ObjectiveId)
                return;

            quest.ObjectiveUpdate(info.Id, 1u);
        }
    }
}
