namespace NexusForever.Game.Quest
{
    internal static class ShadeEveQuestSeason
    {
        private const int ResetHourUtc = 10;

        internal static bool IsIntroductoryQuest(uint questId) => questId is 9760u or 9761u;

        internal static bool CanAccept(uint questId, DateTime now)
        {
            if (questId is not (9688u or 9689u or 9760u or 9761u))
                return true;

            var start = new DateTime(now.Year, 10, 19, ResetHourUtc, 0, 0, DateTimeKind.Utc);
            var end = new DateTime(now.Year, 11, 2, ResetHourUtc, 0, 0, DateTimeKind.Utc);
            return now >= start && now < end;
        }
    }
}
