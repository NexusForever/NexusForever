using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingQueueLeave)]
    public class ServerMatchingQueueLeave : IWritable
    {
        public uint Unknown { get; set; }
        public Game.Static.Matching.MatchType Unknown4 { get; set; }
        public Game.Static.Matching.MatchType Unknown8 { get; set; }
        public NetworkBitArray Mask { get; set; } = new NetworkBitArray(16, NetworkBitArray.BitOrder.LeastSignificantBit);

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown, 4u);
            writer.Write(Unknown4, 5u);
            writer.Write(Unknown8, 5u);
            writer.WriteBytes(Mask.GetBuffer());
        }
    }
}
