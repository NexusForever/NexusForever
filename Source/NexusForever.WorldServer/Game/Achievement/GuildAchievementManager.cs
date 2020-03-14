using System.Collections.Generic;
using NexusForever.WorldServer.Game.Achievement.Static;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Achievement
{
    // TODO: finish implementing this when guild PR is accepted
    public class GuildAchievementManager : BaseAchievementManager
    {
        // private Guild guild;

        protected override Achievement CreateAchievement(AchievementInfo info)
        {
            return new GuildAchievement(/*guild.Id*/123, info);
        }

        protected override void SendAchievementUpdate(IEnumerable<Achievement> updates)
        {
            /*foreach (Player member in guild)
                SendAchievementUpdate(member, grants);*/
        }

        public override void CheckAchievements(Player target, AchievementType type, uint objectId, uint objectIdAlt = 0, uint count = 1)
        {
            CheckAchievements(target, GlobalAchievementManager.Instance.GetGuildAchievements(type), objectId, objectIdAlt, count);
        }
    }
}
