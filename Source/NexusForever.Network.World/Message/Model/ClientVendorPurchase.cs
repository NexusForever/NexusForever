using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientVendorPurchase)]
    public class ClientVendorPurchase : IReadable
    {
        public uint VendorIndex { get; set; }
        public uint VendorItemQty { get; set; }

        public void Read(GamePacketReader reader)
        {
            VendorIndex = reader.ReadUInt();
            VendorItemQty = reader.ReadUInt();
        }
    }
}
