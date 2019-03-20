using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Guild
{
    public class GuildInvite
    {
        public ulong GuildId { get; set; }
        public ulong InviteeId { get; set; }

        /// <summary>
        /// Create a new <see cref="GuildInvite"/>
        /// </summary>
        public GuildInvite() { }
    }
}
