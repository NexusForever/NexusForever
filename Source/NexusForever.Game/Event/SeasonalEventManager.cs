using System.Collections.Immutable;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Event;

namespace NexusForever.Game.Event
{
    public sealed class SeasonalEventManager(TimeProvider timeProvider) : ISeasonalEventManager
    {
        private ImmutableDictionary<uint, ImmutableArray<Schedule>> schedulesByQuest =
            ImmutableDictionary<uint, ImmutableArray<Schedule>>.Empty;

        public void Initialise(IEnumerable<SeasonalEventModel> events)
        {
            schedulesByQuest = events.Select(model => (Model: model, Schedule: CreateSchedule(model)))
                .SelectMany(row => row.Model.Quests.Select(quest => (quest.QuestId, row.Schedule)))
                .GroupBy(row => row.QuestId, row => row.Schedule)
                .ToImmutableDictionary(group => group.Key, group => group.ToImmutableArray());
        }

        public bool IsQuestAvailable(uint questId)
        {
            if (!schedulesByQuest.TryGetValue(questId, out var schedules))
                return true;

            DateTimeOffset now = timeProvider.GetUtcNow();
            return schedules.Any(schedule => schedule.Contains(now));
        }

        public bool IsEntityInQuestEvent(uint entityId, uint questId) =>
            schedulesByQuest.TryGetValue(questId, out var schedules) &&
            schedules.Any(schedule => schedule.EntityIds.Contains(entityId));

        private static Schedule CreateSchedule(SeasonalEventModel model)
        {
            if (model.StartMonth is < 1 or > 12 ||
                model.StartDay == 0 || model.StartDay > DateTime.DaysInMonth(2000, model.StartMonth) ||
                model.StartHourUtc > 23 || model.DurationHours is 0 or > 366 * 24)
                throw new InvalidOperationException($"Seasonal event {model.Id} has an invalid annual UTC schedule.");

            return new Schedule(model.StartMonth, model.StartDay, model.StartHourUtc,
                TimeSpan.FromHours(model.DurationHours), model.Entities.Select(entity => entity.EntityId).ToImmutableHashSet());
        }

        private sealed record Schedule(byte Month, byte Day, byte Hour, TimeSpan Duration, ImmutableHashSet<uint> EntityIds)
        {
            public bool Contains(DateTimeOffset now) =>
                Contains(now, now.Year) || (now.Year > 1 && Contains(now, now.Year - 1));

            private bool Contains(DateTimeOffset now, int year)
            {
                // A February 29 event has no occurrence in a non-leap year.
                if (Day > DateTime.DaysInMonth(year, Month))
                    return false;

                var start = new DateTimeOffset(year, Month, Day, Hour, 0, 0, TimeSpan.Zero);
                return now >= start && now - start < Duration;
            }
        }
    }
}
