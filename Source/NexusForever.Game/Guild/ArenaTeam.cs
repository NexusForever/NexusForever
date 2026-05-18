using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Static.Guild;
using NexusForever.Network.Internal;

namespace NexusForever.Game.Guild
{
    public class ArenaTeam : GuildBase, IArenaTeam
    {
        public override GuildType Type => type;
        private GuildType type;

        public override uint MaxMembers
        {
            get => Type switch
            {
                GuildType.ArenaTeam2v2 => 2u,
                GuildType.ArenaTeam3v3 => 3u,
                GuildType.ArenaTeam5v5 => 5u,
                _ => throw new InvalidOperationException()
            };
        }

        #region Dependency Injection

        public ArenaTeam(
            IRealmContext realmContext,
            IInternalMessagePublisher messagePublisher)
            : base(realmContext, messagePublisher)
        {
        }

        #endregion

        /// <summary>
        /// Create a new <see cref="IArenaTeam"/> using supplied parameters.
        /// </summary>
        public void Initialise(GuildType type, string guildName, string leaderRankName, string councilRankName, string memberRankName)
        {
            this.type = type;

            Initialise(guildName, leaderRankName, councilRankName, memberRankName);
        }
    }
}
