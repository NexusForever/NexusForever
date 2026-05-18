using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Achievement
{
    public interface IAchievementInfo
    {
        ushort Id { get; }
        AchievementEntry Entry { get; }
        List<AchievementChecklistEntry> ChecklistEntries { get; }

        bool IsPlayerAchievement { get; }
    }
}