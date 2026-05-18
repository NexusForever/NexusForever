using NexusForever.Game.Static.Achievement;

namespace NexusForever.Game.Abstract.Achievement
{
    public interface IGlobalAchievementManager
    {
        void Initialise();

        /// <summary>
        /// Return <see cref="IAchievementInfo"/> for supplied achievement id.
        /// </summary>
        IAchievementInfo GetAchievement(ushort id);

        /// <summary>
        /// Return all <see cref="IAchievementInfo"/>'s of <see cref="AchievementType"/> that can be completed by a player.
        /// </summary>
        IEnumerable<IAchievementInfo> GetCharacterAchievements(AchievementType type);

        /// <summary>
        /// Return all <see cref="IAchievementInfo"/>'s of <see cref="AchievementType"/> that can be completed by a guild.
        /// </summary>
        IEnumerable<IAchievementInfo> GetGuildAchievements(AchievementType type);
    }
}