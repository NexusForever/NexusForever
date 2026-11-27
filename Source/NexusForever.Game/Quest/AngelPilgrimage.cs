using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Quest;
using NexusForever.GameTable;

namespace NexusForever.Game.Quest
{
    public static class AngelPilgrimage
    {
        private readonly record struct Memorial(uint Creature, uint ExileObjective, uint ExileMarker,
            uint DominionObjective, uint DominionMarker);

        private const float InteractionRangeSquared = 36f;
        private const float MarkerToleranceSquared = 16f;

        public static void RecordEmote(IPlayer player, ushort emoteId, uint? targetGuid)
        {
            Memorial? memorial = emoteId switch
            {
                20 => new(65411, 18572, 47581, 18563, 47309), // kneel
                5  => new(65412, 18573, 47582, 18564, 47310), // drink
                31 => new(65413, 18574, 47583, 18565, 47311), // cry
                27 => new(65415, 18575, 47584, 18566, 47312), // bow
                _  => null
            };
            if (memorial is not { } site || !player.InWorld || player.IsLoading || !player.IsAlive ||
                player.Map?.Entry is not { } world)
                return;

            (ushort questId, uint objectiveId, uint markerId) = (world.Id, player.Faction1) switch
            {
                (51u, Faction.Exile) => ((ushort)9688, site.ExileObjective, site.ExileMarker),
                (22u, Faction.Dominion) => ((ushort)9689, site.DominionObjective, site.DominionMarker),
                _ => ((ushort)0, 0u, 0u)
            };
            if (questId == 0u)
                return;

            IQuest quest = player.QuestManager.GetActiveQuests().FirstOrDefault(candidate =>
                candidate.Id == questId && candidate.State == QuestState.Accepted && !candidate.PendingDelete);
            IQuestObjective goal = quest?.FirstOrDefault(candidate => candidate.ObjectiveInfo.Id == objectiveId);
            if (goal?.ObjectiveInfo is not { Type: QuestObjectiveType.ActivateEntity, Entry:
                    { Count: 1u, Data: 0u } nativeGoal } || nativeGoal.WorldLocationsIdIndicator00 != markerId ||
                goal.IsComplete() ||
                GameTableManager.Instance.WorldLocation2.GetEntry(markerId) is not { } marker ||
                marker.WorldId != world.Id)
                return;

            var position = new Vector3(marker.Position0, marker.Position1, marker.Position2);
            bool atMemorial = player.GetVisibleCreature<IWorldEntity>(site.Creature).Any(actor =>
                actor.EntityId > 0u && actor.Map == player.Map && actor.Zone?.Id == marker.WorldZoneId &&
                (targetGuid == null || targetGuid == actor.Guid) &&
                Vector3.DistanceSquared(actor.Position, position) <= MarkerToleranceSquared &&
                Vector3.DistanceSquared(actor.Position, player.Position) <= InteractionRangeSquared);
            if (atMemorial)
                quest.ObjectiveUpdate(objectiveId, 1u);
        }
    }
}
