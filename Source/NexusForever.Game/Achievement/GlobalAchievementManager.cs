using NexusForever.Game.Abstract.Achievement;
using NexusForever.Game.Static.Achievement;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.Achievement
{
    public sealed class GlobalAchievementManager : Singleton<GlobalAchievementManager>, IGlobalAchievementManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<ushort, IAchievementInfo> achievements = new();
        private readonly Dictionary<AchievementType, List<IAchievementInfo>> characterAchievements = new();
        private readonly Dictionary<AchievementType, List<IAchievementInfo>> guildAchievements = new();

        private GlobalAchievementManager()
        {
        }

        public void Initialise()
        {
            DateTime start = DateTime.UtcNow;

            foreach (AchievementEntry entry in GameTableManager.Instance.Achievement.Entries)
            {
                var info = new AchievementInfo(entry);
                achievements.Add((ushort)entry.Id, info);

                AchievementType type = (AchievementType)entry.AchievementTypeId;
                Dictionary<AchievementType, List<IAchievementInfo>> collection = info.IsPlayerAchievement ? characterAchievements : guildAchievements;
                if (!collection.ContainsKey(type))
                    collection.Add(type, new List<IAchievementInfo>());

                collection[type].Add(new AchievementInfo(entry));
            }

            TimeSpan span = DateTime.UtcNow - start;
            log.Info($"Initialised {achievements.Count} achievements in {span.TotalMilliseconds}ms.");
        }

        /// <summary>
        /// Return <see cref="IAchievementInfo"/> for supplied achievement id.
        /// </summary>
        public IAchievementInfo GetAchievement(ushort id)
        {
            return achievements.TryGetValue(id, out IAchievementInfo info) ? info : null;
        }

        /// <summary>
        /// Return all <see cref="IAchievementInfo"/>'s of <see cref="AchievementType"/> that can be completed by a player.
        /// </summary>
        public IEnumerable<IAchievementInfo> GetCharacterAchievements(AchievementType type)
        {
            if (!characterAchievements.TryGetValue(type, out List<IAchievementInfo> achievements))
                return Enumerable.Empty<IAchievementInfo>();

            return achievements;
        }

        /// <summary>
        /// Return all <see cref="AchievementInfo"/>'s of <see cref="AchievementType"/> that can be completed by a guild.
        /// </summary>
        public IEnumerable<IAchievementInfo> GetGuildAchievements(AchievementType type)
        {
            if (!guildAchievements.TryGetValue(type, out List<IAchievementInfo> achievements))
                return Enumerable.Empty<IAchievementInfo>();

            return achievements;
        }
    }
}
