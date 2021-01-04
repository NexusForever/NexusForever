using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Guild.Static;

namespace NexusForever.WorldServer.Game.Guild
{
    public class Circle : GuildBase
    {
        public override uint MaxMembers => 20u;

        /// <summary>
        /// Create a new <see cref="Circle"/> using an existing database model.
        /// </summary>
        public Circle(GuildModel baseModel) 
            : base(baseModel)
        {
        }

        /// <summary>
        /// Create a new <see cref="Circle"/> using the supplied parameters.
        /// </summary>
        public Circle(string name, string leaderRankName, string councilRankName, string memberRankName)
            : base(GuildType.Circle, name, leaderRankName, councilRankName, memberRankName)
        {
        }
    }
}
