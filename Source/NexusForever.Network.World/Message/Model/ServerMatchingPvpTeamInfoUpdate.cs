using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Send whenever the information for a team in a PvP match is updated during a match. This includes when a team first joins a match.
    [Message(GameMessageOpcode.ServerMatchingPvpTeamInfoUpdate)]
    public class ServerMatchingPvpTeamInfoUpdate : IWritable
    {
        public string Team1Name { get; set; }
        public string Team2Name { get; set; }
        public uint Team1Rating { get; set; }
        public uint Team2Rating { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(Team1Name);
            writer.WriteStringWide(Team2Name);
            writer.Write(Team1Rating);
            writer.Write(Team2Rating);
        }
    }
}
