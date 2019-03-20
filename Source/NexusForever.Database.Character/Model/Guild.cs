using System;
using System.Collections.Generic;

namespace NexusForever.Database.Character.Model
{
    public partial class Guild
    {
        public Guild()
        {
            GuildRank = new HashSet<GuildRank>();
            GuildMember = new HashSet<GuildMember>();
        }

        public ulong Id { get; set; }
        public byte Type { get; set; }
        public string Name { get; set; }
        public ulong LeaderId { get; set; }
        public DateTime CreateTime { get; set; }

        public ICollection<GuildRank> GuildRank { get; set; }
        public ICollection<GuildMember> GuildMember { get; set; }
        public GuildData GuildData { get; set; }
    }
}
