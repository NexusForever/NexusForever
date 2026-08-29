using NexusForever.Game.Static.Guild;

namespace NexusForever.Game.Abstract.Guild
{
    public interface IArenaTeam : IGuildBase
    {
        /// <summary>
        /// Create a new <see cref="IArenaTeam"/> using supplied parameters.
        /// </summary>
        public void Initialise(GuildType type, string guildName, string leaderRankName, string councilRankName, string memberRankName);
    }
}