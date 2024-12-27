using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingMatchReady)]
    public class ServerMatchingMatchReady : IWritable
    {
        public Game.Static.Matching.MatchType MatchType { get; set; }
        public uint TotalAllies { get; set; }
        public uint TotalEnemies { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MatchType, 5u);
            writer.Write(TotalAllies);
            writer.Write(TotalEnemies);
        }
    }
}
