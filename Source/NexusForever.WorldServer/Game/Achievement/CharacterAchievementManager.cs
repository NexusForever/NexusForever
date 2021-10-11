using System.Collections.Generic;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Achievement.Static;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Achievement
{
    public sealed class CharacterAchievementManager : BaseAchievementManager<CharacterAchievementModel>
    {
        private readonly Player owner;
        protected override ulong OwnerId => owner.CharacterId;

        /// <summary>
        /// Create a new <see cref="CharacterAchievementManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public CharacterAchievementManager(Player owner, CharacterModel model)
        {
            this.owner = owner;
            Initialise(model.Achievement, true);
        }

        /// <summary>
        /// Send initial <see cref="Achievement{T}"/> information to owner on login.
        /// </summary>
        /// <remarks>
        /// Guild achievements will also be sent if owner is part of a <see cref="Guild.Guild"/>.
        /// </remarks>
        public override void SendInitialPackets(Player _)
        {
            base.SendInitialPackets(owner);
            owner.GuildManager.Guild?.AchievementManager.SendInitialPackets(owner);
        }

        protected override void SendAchievementUpdate(IEnumerable<Achievement<CharacterAchievementModel>> updates)
        {
            owner.Session.EnqueueMessageEncrypted(BuildAchievementUpdate(updates));
        }

        /// <summary>
        /// Update or complete player achievements of <see cref="AchievementType"/> as <see cref="Player"/> with supplied object ids.
        /// </summary>
        public override void CheckAchievements(Player target, AchievementType type, uint objectId, uint objectIdAlt = 0u, uint count = 1u)
        {
            CheckAchievements(target, GlobalAchievementManager.Instance.GetCharacterAchievements(type), objectId, objectIdAlt, count);
            target.GuildManager.Guild?.AchievementManager.CheckAchievements(target, type, objectId, objectIdAlt, count);
        }

        protected override void CompleteAchievement(Achievement<CharacterAchievementModel> achievement)
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
