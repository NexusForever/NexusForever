using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Network.World.Message.Model.Guild
{
    public class GuildMember : IWritable
    {
        public Identity PlayerIdentity { get; set; }
        public uint Rank { get; set; }
        public string Name { get; set; }
        public Sex Sex { get; set; }
        public Class Class { get; set; }
        public Path Path { get; set; }
        public uint Level { get; set; }
        public float LastLogoutTimeDays { get; set; }
        public uint PvpWins { get; set; }
        public uint PvpLosses { get; set; }
        public uint PvpDraws { get; set; }
        public string Note { get; set; }
        public uint RecruitmentAvailability { get; set; } // Only bit 2 is used
        public int CommunityReservedPlotIndex { get; set; } = -1;

        public void Write(GamePacketWriter writer)
        {
            PlayerIdentity.Write(writer);
            writer.Write(Rank);
            writer.WriteStringWide(Name);
            writer.Write(Sex, 2u);
            writer.Write(Class, 32u);
            writer.Write(Path, 32u);
            writer.Write(Level);
            writer.Write(LastLogoutTimeDays);
            writer.Write(PvpWins);
            writer.Write(PvpLosses);
            writer.Write(PvpDraws);
            writer.WriteStringWide(Note);
            writer.Write(RecruitmentAvailability);
            writer.Write(CommunityReservedPlotIndex);
        }
    }
}
