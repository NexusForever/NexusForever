using NexusForever.Game.Static.Guild;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Guild
{
    public class GuildData : IWritable
    {
        public class Info : IWritable
        {
            public class PvPRating : IWritable
            {
                public uint Wins { get; set; }
                public uint Losses { get; set; }
                public uint Draws { get; set; }
                public uint Rating { get; set; }
                public uint PvpSeason { get; set; }
                public uint KFactor { get; set; }

                public void Write(GamePacketWriter writer)
                {
                    writer.Write(Wins);
                    writer.Write(Losses);
                    writer.Write(Draws);
                    writer.Write(Rating);
                    writer.Write(PvpSeason);
                    writer.Write(KFactor);
                }
            }

            public string MessageOfTheDay { get; set; }
            public string GuildInfo { get; set; }
            public GuildQueueState QueueState { get; set; }
            public PvPRating PvPRatings { get; set; } = new();
            public float GuildCreationDateInDays { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.WriteStringWide(MessageOfTheDay);
                writer.WriteStringWide(GuildInfo);
                writer.Write(QueueState, 3u);
                PvPRatings.Write(writer);
                writer.Write(GuildCreationDateInDays);
            }
        }

        public class ActivePerk : IWritable
        {
            public GuildPerk Perk { get; set; } // Is the guildPerkId, see guildPerk tbl or GuildPerk enum
            public float EndTime { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Perk, 14u);
                writer.Write(EndTime);
            }
        }

        public ulong GuildId { get; set; }
        public string GuildName { get; set; }
        public GuildFlag Flags { get; set; }
        public GuildType Type { get; set; }

        public List<GuildRank> Ranks { get; set; } = new(new GuildRank[10]);

        public GuildStandard GuildStandard { get; set; } = new();

        public uint MemberCount { get; set; }
        public uint OnlineMemberCount { get; set; }
        public uint Influence { get; set; }
        public uint BonusInfluenceRemaining { get; set; }

        public ulong Money { get; set; }
        public uint WarCoins { get; set; }
        public uint BankTabCount { get; set; }
        public List<string> BankTabNames { get; set; } = new(new string[10]);

        public ulong[] UnlockedPerks { get; set; } = new ulong[2];  // Each ulong is a set of flags represeting the unlock state of a GuildPerkId
                                                                    // GuildPerkId = 1 is bit 0 of the first ulong and the rest follow upwards joining
                                                                    // to the next ulong when GuildPerkId > 64

        public List<ActivePerk> ActivePerks { get; set; } = new();

        public Info GuildInfo { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GuildId);
            writer.WriteStringWide(GuildName);
            writer.Write(Flags, 32u);
            writer.Write(Type, 4u);

            if (Ranks.Count < 10)
                for (int i = Ranks.Count; i < 10; i++)
                    Ranks.Add(new GuildRank());
            Ranks.ForEach(c => c.Write(writer));

            GuildStandard.Write(writer);

            writer.Write(MemberCount);
            writer.Write(OnlineMemberCount);
            writer.Write(Influence);
            writer.Write(BonusInfluenceRemaining);
            writer.Write(Money);
            writer.Write(WarCoins);
            writer.Write(BankTabCount);

            foreach (string str in BankTabNames)
                writer.WriteStringWide(str);

            foreach (ulong perk in UnlockedPerks)
                writer.Write(perk);

            writer.Write(ActivePerks.Count);
            ActivePerks.ForEach(c => c.Write(writer));

            GuildInfo.Write(writer);
        }
    }
}
