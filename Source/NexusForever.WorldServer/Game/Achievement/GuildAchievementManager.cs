using System.Collections.Generic;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Achievement.Static;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Achievement
{
    public sealed class GuildAchievementManager : BaseAchievementManager<GuildAchievementModel>
    {
        private readonly Guild.Guild guild;
        protected override ulong OwnerId => guild.Id;

        /// <summary>
        /// Create a new <see cref="CharacterAchievementManager"/> from existing <see cref="GuildModel"/> database model.
        /// </summary>
        public GuildAchievementManager(Guild.Guild guild, GuildModel model)
        {
            this.guild = guild;
            Initialise(model.Achievement, false);
        }

        /// <summary>
        /// Create a new <see cref="CharacterAchievementManager"/> for <see cref="Guild.Guild"/>.
        /// </summary>
        public GuildAchievementManager(Guild.Guild guild)
        {
            this.guild = guild;
        }

        protected override void SendAchievementUpdate(IEnumerable<Achievement<GuildAchievementModel>> updates)
        {
            guild.Broadcast(BuildAchievementUpdate(updates));
        }

        /// <summary>
        /// Update or complete player achievements of <see cref="AchievementType"/> as <see cref="Player"/> with supplied object ids.
        /// </summary>
        public override void CheckAchievements(Player target, AchievementType type, uint objectId, uint objectIdAlt = 0, uint count = 1)
        {
            CheckAchievements(target, GlobalAchievementManager.Instance.GetGuildAchievements(type), objectId, objectIdAlt, count);
        }
    }
}
