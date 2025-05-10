using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Send whenever the information for a team in a PvP match is updated during a match. This includes when a team first joins a match.
    [Message(GameMessageOpcode.ServerMatchingPvpTeamInfoUpdate)]
    public class ServerMatchingPvpTeamInfoUpdate : IWritable
    {
        public string TeamName { get; set; }
        public uint Rating1 { get; set; }
        public uint Rating2 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(TeamName);
            writer.Write(Rating1);
            writer.Write(Rating2);
        }
    }
}
