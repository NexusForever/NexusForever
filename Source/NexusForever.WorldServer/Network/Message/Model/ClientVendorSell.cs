using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientSellItemToVendor, MessageDirection.Client)]
    public class ClientVendorSell : IReadable
    {
        public ItemLocation ItemLocation { get; } = new ItemLocation();
        public uint Count { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemLocation.Read(reader);
            Count = (uint)reader.ReadInt();
        }
    }
}
