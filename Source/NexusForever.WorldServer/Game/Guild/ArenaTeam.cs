using System;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Guild.Static;

namespace NexusForever.WorldServer.Game.Guild
{
    public class ArenaTeam : GuildBase
    {
        public override uint MaxMembers
        {
            get
            {
                return Type switch
                {
                    GuildType.ArenaTeam2v2 => 2u,
                    GuildType.ArenaTeam3v3 => 3u,
                    GuildType.ArenaTeam5v5 => 5u,
                    _ => throw new InvalidOperationException()
                };
            }
        }

        /// <summary>
        /// Create a new <see cref="ArenaTeam"/> using <see cref="GuildModel"/>
        /// </summary>
        public ArenaTeam(GuildModel baseModel) 
            : base(baseModel)
        {
        }

        /// <summary>
        /// Create a new <see cref="ArenaTeam"/> using the supplied parameters.
        /// </summary>
        public ArenaTeam(GuildType type, string name, string leaderRankName, string councilRankName, string memberRankName)
            : base(type, name, leaderRankName, councilRankName, memberRankName)
        {
        }
    }
}
