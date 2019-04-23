using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientSellItemToVendor)]
    public class ClientVendorSell : IReadable
    {
        public ItemLocation ItemLocation { get; } = new ItemLocation();
        public uint Quantity { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemLocation.Read(reader);
            Quantity = reader.ReadUInt();
        }
    }
}
