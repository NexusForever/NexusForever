using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingMatchReady)]

    // Specifically sent for a match that has not started yet
    public class ServerMatchingMatchReady : IWritable
    {
        public Game.Static.Matching.MatchType MatchType { get; set; }
        public uint PendingAllies { get; set; }
        public uint PendingEnemies { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MatchType, 5u);
            writer.Write(PendingAllies);
            writer.Write(PendingEnemies);
        }
    }
}
