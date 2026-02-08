using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildPvpStatsUpdate)]
    public class ServerGuildPvpStatsUpdate : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public uint Wins { get; set; }
        public uint Losses { get; set; }
        public uint Draws { get; set; }
        public uint Rating { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(Wins);
            writer.Write(Losses);
            writer.Write(Draws);
            writer.Write(Rating);
        }
    }
}
