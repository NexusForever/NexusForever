using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class PublicEventStats : IWritable
    {
        public NetworkBitArray Mask { get; set; } = new NetworkBitArray(32, NetworkBitArray.BitOrder.LeastSignificantBit);
        public List<uint> Values { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.WriteBytes(Mask.GetBuffer());
            writer.Write(Values.Count, 5);

            foreach (uint value in Values)
                writer.Write(value);
        }
    }
}
