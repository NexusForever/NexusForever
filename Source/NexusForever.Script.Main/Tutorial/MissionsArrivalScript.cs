using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Script.Template;

namespace NexusForever.Script.Main.Tutorial
{
    public sealed class MissionsArrivalScript : IWorldEntityScript, IOwnedScript<IPlayer>
    {
        internal const uint TutorialWorld = 3460u;
        internal const uint MissionsZone = 4964u;
        internal const uint MissionsArea = 5968u;
        internal const uint ProjectorMarker = 51713u;

        public void OnLoad(IPlayer owner)
        {
        }

        public void OnEnterZone(IWorldEntity entity, uint zone)
        {
            if (zone != MissionsArea || entity is not IPlayer
                { Map.Entry.Id: TutorialWorld, Zone.ParentZoneId: MissionsZone } player)
                return;

            IQuest quest = GetQuest(player, out uint objectiveId);
            if (quest?.State == QuestState.Accepted)
                quest.ObjectiveUpdate(objectiveId, 1u);
        }

        internal static IQuest GetQuest(IPlayer player, out uint objectiveId)
        {
            (ushort questId, uint goalId) = player.Faction1 == player.Faction2 ? player.Faction1 switch
            {
                Faction.Exile => ((ushort)10540, 21373u),
                Faction.Dominion => ((ushort)10541, 21376u),
                _ => default
            } : default;

            objectiveId = goalId;
            if (questId == 0u)
                return null;

            IQuest quest = player.QuestManager?.GetActiveQuests().FirstOrDefault(active
                => active.Id == questId && !active.PendingDelete && active.State is QuestState.Accepted or QuestState.Achieved);
            IQuestObjectiveInfo goal = quest?.Info.Objectives.FirstOrDefault(candidate => candidate.Id == goalId);
            return goal is { Type: QuestObjectiveType.EnterZone, Entry: { Data: MissionsZone, Count: 1u } entry }
                && entry.WorldLocationsIdIndicator00 == ProjectorMarker ? quest : null;
        }
    }
}
