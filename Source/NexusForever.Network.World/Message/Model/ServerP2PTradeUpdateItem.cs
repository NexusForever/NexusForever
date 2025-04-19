using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerP2PTradeUpdateItem)]
    public class ServerP2PTradeUpdateItem : IWritable
    {
        public uint TradeIndex { get; set; }
        public uint OwnerUnitId { get; set; }
        public uint Item2Id { get; set; }
        public ulong ItemGuid { get; set; }
        public uint Quantity { get; set; }
        public ulong Unknown1_64bit { get; set; }
        public uint Unknown2_32bit { get; set; }
        public ulong Unknown3_64bit { get; set; }
        public uint Unknown4_18bit { get; set; }
        public uint[] UnknownArray { get; set; } = new uint[5];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TradeIndex);
            writer.Write(OwnerUnitId);
            writer.Write(Item2Id, 18);
            writer.Write(ItemGuid);
            writer.Write(Quantity);
            writer.Write(Unknown1_64bit);
            writer.Write(Unknown2_32bit);
            writer.Write(Unknown3_64bit);
            writer.Write(Unknown4_18bit, 18);
            foreach (var unknown in UnknownArray)
            {
                writer.Write(unknown);
            }
        }
    }
}
