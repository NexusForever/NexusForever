using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Achievement.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Achievement
{
    public static class GlobalAchievementManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static readonly Dictionary<ushort, AchievementInfo> achievements
            = new Dictionary<ushort, AchievementInfo>();
        private static readonly Dictionary<AchievementType, List<AchievementInfo>> characterAchievements
            = new Dictionary<AchievementType, List<AchievementInfo>>();
        private static readonly Dictionary<AchievementType, List<AchievementInfo>> guildAchievements
            = new Dictionary<AchievementType, List<AchievementInfo>>();

        public static void Initialise()
        {
            DateTime start = DateTime.UtcNow;

            foreach (AchievementEntry entry in GameTableManager.Achievement.Entries)
            {
                var info = new AchievementInfo(entry);
                achievements.Add((ushort)entry.Id, info);

                AchievementType type = (AchievementType)entry.AchievementTypeId;
                IDictionary<AchievementType, List<AchievementInfo>> collection = info.IsPlayerAchievement ? characterAchievements : guildAchievements;
                if (!collection.ContainsKey(type))
                    collection.Add(type, new List<AchievementInfo>());

                collection[type].Add(new AchievementInfo(entry));
            }

            TimeSpan span = DateTime.UtcNow - start;
            log.Info($"Initialised {achievements.Count} achievements in {span.TotalMilliseconds}ms.");
        }

        /// <summary>
        /// Return <see cref="AchievementInfo"/> for supplied achievement id.
        /// </summary>
        public static AchievementInfo GetAchievement(ushort id)
        {
            return achievements.TryGetValue(id, out AchievementInfo info) ? info : null;
        }

        /// <summary>
        /// Return all <see cref="AchievementInfo"/>'s of <see cref="AchievementType"/> that can be completed by a player.
        /// </summary>
        public static IEnumerable<AchievementInfo> GetCharacterAchievements(AchievementType type)
        {
            if (!characterAchievements.TryGetValue(type, out List<AchievementInfo> achievements))
                return Enumerable.Empty<AchievementInfo>();

            return achievements;
        }

        /// <summary>
        /// Return all <see cref="AchievementInfo"/>'s of <see cref="AchievementType"/> that can be completed by a guild.
        /// </summary>
        public static IEnumerable<AchievementInfo> GetGuildAchievements(AchievementType type)
        {
            if (!guildAchievements.TryGetValue(type, out List<AchievementInfo> achievements))
                return Enumerable.Empty<AchievementInfo>();

            return achievements;
        }
    }
}
