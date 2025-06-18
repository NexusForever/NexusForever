using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Updates the wait time for a specific match type
    [Message(GameMessageOpcode.ServerMatchingAverageWaitTimeUpdate)]
    public class ServerMatchingAverageWaitTimeUpdate : IWritable
    {
        public Game.Static.Matching.MatchType Type { get; set; }
        public uint AverageWaitTime { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type,5u);
            writer.Write(AverageWaitTime);
        }
    }
}
