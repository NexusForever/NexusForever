using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Guild.Static;

namespace NexusForever.WorldServer.Game.Guild
{
    public class WarParty : GuildBase
    {
        public override uint MaxMembers => 30u;

        /// <summary>
        /// Create a new <see cref="WarParty"/> using <see cref="GuildModel"/>
        /// </summary>
        public WarParty(GuildModel baseModel) 
            : base(baseModel)
        {
        }

        /// <summary>
        /// Create a new <see cref="WarParty"/> using the supplied parameters.
        /// </summary>
        public WarParty(string name, string leaderRankName, string councilRankName, string memberRankName)
            : base(GuildType.WarParty, name, leaderRankName, councilRankName, memberRankName)
        {
        }
    }
}
