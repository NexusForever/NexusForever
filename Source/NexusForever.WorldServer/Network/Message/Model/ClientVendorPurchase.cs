using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
