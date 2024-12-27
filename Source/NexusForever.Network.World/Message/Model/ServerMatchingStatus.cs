using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingStatus)]
    public class ServerMatchingStatus : IWritable
    {
        public uint Unknown { get; set; }
        public Game.Static.Matching.MatchType MatchType { get; set; }
        public Game.Static.Matching.MatchType Unknown8 { get; set; }
        public NetworkBitArray Mask { get; set; } = new NetworkBitArray(16, NetworkBitArray.BitOrder.LeastSignificantBit);

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown, 4u);
            writer.Write(MatchType, 5u);
            writer.Write(Unknown8, 5u);
            writer.WriteBytes(Mask.GetBuffer());
        }
    }
}
