using System;
using NexusForever.Database.Character;

namespace NexusForever.WorldServer.Game.Achievement
{
    // TODO: finish implementing this when guild PR is accepted
    public class GuildAchievement : Achievement
    {
        private ulong guildId;

        public GuildAchievement(ulong guildId, AchievementInfo info)
            : base(info, 0u, 0u, null, true)
        {
            this.guildId = guildId;
        }

        public override void Save(CharacterContext context)
        {
            throw new NotImplementedException();
        }
    }
}
