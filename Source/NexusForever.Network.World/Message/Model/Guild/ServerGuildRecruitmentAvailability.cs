using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildRecruitmentAvailability)]
    public class ServerGuildRecruitmentAvailability : IWritable
    {
        public Identity GuildIdentity1 { get; set; }
        public Identity GuildIdentity2 { get; set; } // GuildIdentity2 needs to match GuildIdentity1 for the handler to set the recruitment availability
        public uint RecruitmentAvailability { get; set; } // Bit 2 is the only bit used to signal recruitment availability

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity1.Write(writer);
            GuildIdentity2.Write(writer);
            writer.Write(RecruitmentAvailability);
        }
    }
}
