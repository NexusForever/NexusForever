using NexusForever.Game.Abstract.Achievement;
using NexusForever.Game.Static.Achievement;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Achievement
{
    public class AchievementInfo : IAchievementInfo
    {
        public ushort Id => (ushort)Entry.Id;
        public AchievementEntry Entry { get; }
        public List<AchievementChecklistEntry> ChecklistEntries { get; }

        public bool IsPlayerAchievement => ((AchievementFlags)Entry.Flags & AchievementFlags.Guild) == 0;

        /// <summary>
        /// Create a new <see cref="IAchievementInfo"/> from <see cref="AchievementEntry"/>.
        /// </summary>
        public AchievementInfo(AchievementEntry entry)
        {
            Entry = entry;
            ChecklistEntries = GameTableManager.Instance.AchievementChecklist.Entries
                .Where(t => t.AchievementId == entry.Id)
                .ToList();
        }
    }
}
