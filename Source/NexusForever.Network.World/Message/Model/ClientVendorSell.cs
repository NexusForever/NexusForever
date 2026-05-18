using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientSellItemToVendor)]
    public class ClientVendorSell : IReadable
    {
        public ItemLocation ItemLocation { get; } = new();
        public uint Quantity { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemLocation.Read(reader);
            Quantity = reader.ReadUInt();
        }
    }
}
