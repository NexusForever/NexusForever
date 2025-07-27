using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
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
