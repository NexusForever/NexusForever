using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingQueueStatus)]
    public class ServerMatchingQueueStatus : IWritable
    {
        public MatchingQueueResultShort Result { get; set; }
        public Game.Static.Matching.MatchType MatchType { get; set; }
        public Game.Static.Matching.MatchType Unknown8 { get; set; }
        public NetworkBitArray Mask { get; set; } = new NetworkBitArray(16, NetworkBitArray.BitOrder.LeastSignificantBit);
        // Mask is array of bits to indicate which MatchType is joined, true = joined false = not joined
        // Order of array is same order as Game.Static.Matching.MatchType

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 4u);
            writer.Write(MatchType, 5u);
            writer.Write(Unknown8, 5u);
            writer.WriteBytes(Mask.GetBuffer());
        }
    }
}
