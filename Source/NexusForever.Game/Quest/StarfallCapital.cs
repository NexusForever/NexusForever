using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;

namespace NexusForever.Game.Quest
{
    internal static class StarfallCapital
    {
        private sealed record Capital(Faction Faction, uint Contact, ushort Celebration, uint Goal, uint TargetGroup);

        private static readonly Capital Illium = new(Faction.Dominion, 73057, 10496, 21204, 14307);
        private static readonly Capital Thayd = new(Faction.Exile, 73058, 10534, 21362, 14395);

        private static Capital GetCapital(uint world) => world switch
        {
            22u => Illium,
            51u => Thayd,
            _ => null
        };

        private static bool IsFirework(Capital capital, uint creatureId) =>
            creatureId != 0u &&
            GameTableManager.Instance.TargetGroup.GetEntry(capital.TargetGroup)?.DataEntries.Contains(creatureId) == true;

        internal static bool OwnsQuest(uint id) => id is 10496u or (>= 10502u and <= 10506u) or (>= 10534u and <= 10539u);

        internal static bool CanAccept(uint id, DateTime now)
        {
            if (!OwnsQuest(id))
                return true;
            var start = new DateTime(now.Year, 6, 1, 10, 0, 0, DateTimeKind.Utc);
            if (now < start || now >= start.AddDays(30))
                return false;
            int day = (now - start).Days;
            if (day >= 28)
                return true;
            return id switch
            {
                10503u or 10536u => day < 7,
                10504u or 10537u => day is >= 7 and < 14,
                10505u or 10538u => day is >= 14 and < 21,
                10506u or 10539u => day is >= 21 and < 28,
                _ => true
            };
        }

        internal static DateTime NextReset(DateTime now)
        {
            var start = new DateTime(now.Year, 6, 1, 10, 0, 0, DateTimeKind.Utc);
            return now < start ? start : start.AddYears(1);
        }

        internal static bool CanSee(IPlayer player, IGridEntity entity)
        {
            if (entity is not IWorldEntity { EntityId: > 0u, Map.Entry.Id: var world } actor ||
                GetCapital(world) is not { } capital ||
                (actor.CreatureId != capital.Contact && !IsFirework(capital, actor.CreatureId)))
                return true;

            return player.Faction1 == capital.Faction &&
                (CanAccept(capital.Celebration, DateTime.UtcNow) || player.QuestManager.GetActiveQuests().Any(quest =>
                    OwnsQuest(quest.Id) && !quest.PendingDelete && quest.State is QuestState.Accepted or QuestState.Achieved));
        }

        internal static void ActivateFirework(IPlayer player, ISimpleEntity actor)
        {
            if (actor is not { EntityId: > 0u, Map.Entry.Id: var world, QuestChecklistIdx: < 5 } ||
                GetCapital(world) is not { } capital || !IsFirework(capital, actor.CreatureId) ||
                player.Faction1 != capital.Faction || !player.InWorld || player.IsLoading || !player.IsAlive ||
                !ReferenceEquals(player.Map, actor.Map) || Vector3.DistanceSquared(player.Position, actor.Position) > 4f)
                return;

            IQuest quest = player.QuestManager.GetActiveQuests().FirstOrDefault(q =>
                q.Id == capital.Celebration && q.State == QuestState.Accepted && !q.PendingDelete);
            if (quest?.Any(o => o.ObjectiveInfo.Id == capital.Goal &&
                o.ObjectiveInfo.Type == QuestObjectiveType.ActivateTargetGroupChecklist &&
                o.ObjectiveInfo.Entry.Data == capital.TargetGroup && o.ObjectiveInfo.Entry.Count == 5u) == true)
                quest.ObjectiveUpdate(capital.Goal, 1u << actor.QuestChecklistIdx);
        }
    }
}
