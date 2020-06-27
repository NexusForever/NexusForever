using System.Collections.Generic;
using NexusForever.Database;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Achievement.Static;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Achievement
{
    public class CharacterAchievementManager : BaseAchievementManager
    {
        private readonly Player owner;

        /// <summary>
        /// Create a new <see cref="CharacterAchievementManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public CharacterAchievementManager(Player owner, CharacterModel model)
        {
            this.owner = owner;

            foreach (CharacterAchievementModel achievementModel in model.Achievement)
            {
                AchievementInfo info = GlobalAchievementManager.Instance.GetAchievement(achievementModel.AchievementId);
                if (info == null)
                    throw new DatabaseDataException($"Character {model.Id} has invalid achievement {achievementModel.AchievementId} stored!");

                if (!info.IsPlayerAchievement)
                    throw new DatabaseDataException($"Character {model.Id} has guild achievement {achievementModel.AchievementId} stored!");

                var achievement = new CharacterAchievement(achievementModel, info);
                if (achievement.IsComplete())
                    AchievementPoints += GetAchievementPoints(achievement.Info);

                achievements.Add(achievement.Id, achievement);
            }
        }

        protected override Achievement CreateAchievement(AchievementInfo info)
        {
            return new CharacterAchievement(owner.CharacterId, info);
        }

        /// <summary>
        /// Send initial <see cref="Achievement"/> information to owner on login.
        /// </summary>
        public void SendInitialPackets()
        {
            SendInitialPackets(owner);
        }

        protected override void SendAchievementUpdate(IEnumerable<Achievement> updates)
        {
            SendAchievementUpdate(owner, updates);
        }

        /// <summary>
        /// Update or complete player achievements of <see cref="AchievementType"/> as <see cref="Player"/> with supplied object ids.
        /// </summary>
        public override void CheckAchievements(Player target, AchievementType type, uint objectId, uint objectIdAlt = 0u, uint count = 1u)
        {
            CheckAchievements(target, GlobalAchievementManager.Instance.GetCharacterAchievements(type), objectId, objectIdAlt, count);

            // TODO
            /*if (inGuild)
                guild.AchievementManager.CheckAchievements(target, GlobalAchievementManager.GetGuildAchievements(type), objectId, objectIdAlt, count);*/
        }

        protected override void CompleteAchievement(Achievement achievement)
        {
            base.CompleteAchievement(achievement);

            if (achievement.Info.Entry.CharacterTitleId != 0u)
                owner.TitleManager.AddTitle((ushort)achievement.Info.Entry.CharacterTitleId);

            // TODO
            /*if (isRealmFirst)
            {
                MapManager.BroadcastMessage(new ServerRealmFirstAchievement
                {
                    AchievementId = achievement.Id,
                    Player        = owner.Name
                });
            }*/
        }
    }
}
