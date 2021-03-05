using System;
using System.Collections.Generic;

namespace NexusForever.Database.Character.Model
{
    public class GuildModel
    {
        public ulong Id { get; set; }
        public uint Flags { get; set; }
        public byte Type { get; set; }
        public string Name { get; set; }
        public ulong? LeaderId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? DeleteTime { get; set; }
        public string OriginalName { get; set; }
        public ulong? OriginalLeaderId { get; set; }

        public ICollection<GuildRankModel> GuildRank { get; set; } = new HashSet<GuildRankModel>();
        public ICollection<GuildMemberModel> GuildMember { get; set; } = new HashSet<GuildMemberModel>();
        public ICollection<GuildAchievementModel> Achievement { get; set; } = new HashSet<GuildAchievementModel>();
        public GuildDataModel GuildData { get; set; }
    }
}
