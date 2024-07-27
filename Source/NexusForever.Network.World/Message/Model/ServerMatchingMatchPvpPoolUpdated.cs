using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingMatchPvpPoolUpdated)]
    public class ServerMatchingMatchPvpPoolUpdated : IWritable
    {
        public uint LivesRemainingTeam1 { get; set; }
        public uint LivesRemainingTeam2 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(LivesRemainingTeam1);
            writer.Write(LivesRemainingTeam2);
        }
    }
}
