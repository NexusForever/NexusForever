using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ClientVendorPurchase)]
    public class ClientVendorPurchase : IReadable
    {
        public uint StockUniqueId { get; set; }
        public uint PurchaseQuantity { get; set; }

        public void Read(GamePacketReader reader)
        {
            StockUniqueId = reader.ReadUInt();
            PurchaseQuantity = reader.ReadUInt();
        }
    }
}
