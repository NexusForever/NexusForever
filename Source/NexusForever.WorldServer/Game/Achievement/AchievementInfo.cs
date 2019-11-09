using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Achievement.Static;

namespace NexusForever.WorldServer.Game.Achievement
{
    public class AchievementInfo
    {
        public ushort Id => (ushort)Entry.Id;
        public AchievementEntry Entry { get; }
        public List<AchievementChecklistEntry> ChecklistEntries { get; }

        public bool IsPlayerAchievement => ((AchievementFlags)Entry.Flags & AchievementFlags.Guild) == 0;

        /// <summary>
        /// Create a new <see cref="AchievementInfo"/> from <see cref="AchievementEntry"/>.
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
