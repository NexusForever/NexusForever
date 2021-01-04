using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Guild.Static;

namespace NexusForever.WorldServer.Game.Guild
{
    public class Community : GuildBase
    {
        public override uint MaxMembers => 20u;

        /// <summary>
        /// Create a new <see cref="Community"/> using <see cref="GuildModel"/>
        /// </summary>
        public Community(GuildModel baseModel) 
            : base(baseModel)
        {
        }

        /// <summary>
        /// Create a new <see cref="Community"/> using the supplied parameters.
        /// </summary>
        public Community(string name, string leaderRankName, string councilRankName, string memberRankName)
            : base(GuildType.Community, name, leaderRankName, councilRankName, memberRankName)
        {
        }
    }
}
