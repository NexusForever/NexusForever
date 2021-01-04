using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Guild.Static;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class GuildData: IWritable
    {
        public class Info: IWritable
        {
            public class PvPRating: IWritable
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
            public byte QueueState { get; set; } // 3
            public PvPRating PvPRatings { get; set; } = new PvPRating();
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

        public class ActivePerk: IWritable
        {
            public ushort PerkId { get; set; } // 14
            public float EndTime { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(PerkId, 14u);
                writer.Write(EndTime);
            }
        }

        public ulong GuildId { get; set; }
        public string GuildName { get; set; }
        public GuildFlag Flags { get; set; }
        public GuildType Type { get; set; } // 4

        public List<GuildRank> Ranks { get; set; } = new List<GuildRank>(new GuildRank[10]);

        public GuildStandard GuildStandard { get; set; } = new GuildStandard();

        public uint MemberCount { get; set; }
        public uint OnlineMemberCount { get; set; }
        public uint Influence { get; set; }
        public uint DailyInfluenceRemaining { get; set; }

        public ulong Money { get; set; }
        public uint WarCoins { get; set; }
        public uint BankTabCount { get; set; }
        public List<string> BankTabNames { get; set; } = new List<string>(new string[10]);

        public ulong[] Perks { get; set; } = new ulong[2];
        public List<ActivePerk> ActivePerks { get; set; } = new List<ActivePerk>();

        public Info GuildInfo { get; set; } = new Info();

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
            writer.Write(DailyInfluenceRemaining);
            writer.Write(Money);
            writer.Write(WarCoins);
            writer.Write(BankTabCount);

            foreach (string str in BankTabNames)
                writer.WriteStringWide(str);

            foreach (ulong perk in Perks)
                writer.Write(perk);

            writer.Write(ActivePerks.Count);
            ActivePerks.ForEach(c => c.Write(writer));

            GuildInfo.Write(writer);
        }
    }
}
