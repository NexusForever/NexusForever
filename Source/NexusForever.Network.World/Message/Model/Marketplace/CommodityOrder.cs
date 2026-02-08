using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    public class CommodityOrder : IReadable, IWritable
    {
        public ulong CommodityOrderId { get; set; }
        public uint Item2Id { get; set; }
        public uint Quantity { get; set; }
        public ulong PricePerUnit { get; set; }
        public ulong Price { get; set; }
        public bool IsBuyOrder { get; set; }
        public bool ForceImmediate { get; set; } // Changes the CommodityOrder so it attempts to buy or sell its items for the best price when it is posted.
        public ulong ListTime { get; set; } // is the system _FILETIME struct
        public ulong ExpirationTime { get; set; } // is the system _FILETIME struct

        public void Read(GamePacketReader reader)
        {
            CommodityOrderId = reader.ReadULong();
            Item2Id = reader.ReadUInt(18u);
            Quantity = reader.ReadUInt();
            PricePerUnit = reader.ReadULong();
            Price = reader.ReadULong();
            IsBuyOrder = reader.ReadBit();
            ForceImmediate = reader.ReadBit();
            ListTime = reader.ReadULong();
            ExpirationTime = reader.ReadULong();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CommodityOrderId);
            writer.Write(Item2Id, 18u);
            writer.Write(Quantity);
            writer.Write(PricePerUnit);
            writer.Write(Price);
            writer.Write(IsBuyOrder);
            writer.Write(ForceImmediate);
            writer.Write(ListTime);
            writer.Write(ExpirationTime);
        }
    }
}
