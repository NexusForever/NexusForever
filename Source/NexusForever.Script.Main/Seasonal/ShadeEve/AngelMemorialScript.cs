using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Event;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Seasonal.ShadeEve
{
    [ScriptFilterCreatureId(65411, 65412, 65413, 65415)]
    public sealed class AngelMemorialScript(
        ISeasonalEventManager seasonalEvents,
        IGameTableManager gameTables) : IWorldEntityScript, IOwnedScript<ICreatureEntity>
    {
        private const float InteractionRangeSquared = 36f;
        private const float MarkerToleranceSquared = 16f;

        private ICreatureEntity memorial;

        public void OnLoad(ICreatureEntity owner)
        {
            memorial = owner;
        }

        public void OnPlayerEmote(IPlayer player, ushort emoteId)
        {
            if (memorial is not { EntityId: > 0u } || !player.InWorld || player.IsLoading || !player.IsAlive ||
                !ReferenceEquals(player.Map, memorial.Map) || memorial.Map?.Entry is not { } world ||
                Vector3.DistanceSquared(player.Position, memorial.Position) > InteractionRangeSquared)
                return;

            (ushort expectedEmote, uint exileObjective, uint dominionObjective) = memorial.CreatureId switch
            {
                65411u => ((ushort)20, 18572u, 18563u),
                65412u => ((ushort)5, 18573u, 18564u),
                65413u => ((ushort)31, 18574u, 18565u),
                65415u => ((ushort)27, 18575u, 18566u),
                _ => default
            };
            if (expectedEmote == 0u || emoteId != expectedEmote)
                return;

            (ushort questId, uint objectiveId) = (world.Id, player.Faction1) switch
            {
                (51u, Faction.Exile) => ((ushort)9688, exileObjective),
                (22u, Faction.Dominion) => ((ushort)9689, dominionObjective),
                _ => default
            };
            if (questId == 0u || !seasonalEvents.IsEntityInQuestEvent(memorial.EntityId, questId))
                return;

            IQuest quest = player.QuestManager.GetActiveQuests().FirstOrDefault(candidate =>
                candidate.Id == questId && candidate.State == QuestState.Accepted && !candidate.PendingDelete);
            IQuestObjective objective = quest?.FirstOrDefault(candidate => candidate.ObjectiveInfo.Id == objectiveId);
            if (objective?.ObjectiveInfo is not { Type: QuestObjectiveType.ActivateEntity, Entry: { Count: 1u, Data: 0u } entry } ||
                objective.IsComplete() ||
                gameTables.WorldLocation2.GetEntry(entry.WorldLocationsIdIndicator00) is not { } marker ||
                marker.WorldId != world.Id || memorial.Zone?.Id != marker.WorldZoneId ||
                Vector3.DistanceSquared(memorial.Position, new Vector3(marker.Position0, marker.Position1, marker.Position2)) > MarkerToleranceSquared)
                return;

            quest.ObjectiveUpdate(objectiveId, 1u);
        }
    }
}
