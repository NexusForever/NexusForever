using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingQueueStatus)]
    public class ServerMatchingQueueStatus : IWritable
    {
        public MatchingQueueStatus Status { get; set; }
        public Game.Static.Matching.MatchType JoinedMatchType { get; set; }
        public Game.Static.Matching.MatchType ReadyMatchType { get; set; } // MatchType for active role check on group that members can role check for
        public NetworkBitArray QueuesJoined { get; set; } = new NetworkBitArray(16, NetworkBitArray.BitOrder.LeastSignificantBit);
        // QueuesJoined is array of bits to indicate which MatchType queue is joined, true = joined false = not joined
        // Order of array is same order as Game.Static.Matching.MatchType enum

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Status, 4u);
            writer.Write(JoinedMatchType, 5u);
            writer.Write(ReadyMatchType, 5u);
            writer.WriteBytes(QueuesJoined.GetBuffer());
        }
    }
}
