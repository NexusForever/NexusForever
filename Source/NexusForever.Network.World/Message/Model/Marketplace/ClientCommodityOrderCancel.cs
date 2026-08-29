using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    [Message(GameMessageOpcode.ClientCommodityOrderCancel)]
    public class ClientCommodityOrderCancel : IReadable
    {
        public ulong CommodityOrderId { get; private set; }
        public uint Item2Id { get; private set; }
        public bool IsBuyOrder { get; private set; }

        public void Read(GamePacketReader reader)
        {
            CommodityOrderId = reader.ReadULong();
            Item2Id = reader.ReadUInt(18u);
            IsBuyOrder = reader.ReadBit();
        }
    }
}
