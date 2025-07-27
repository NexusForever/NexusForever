using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ClientBuybackItemFromVendor)]
    public class ClientBuybackItemFromVendor : IReadable
    {
        public uint UniqueId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            UniqueId = reader.ReadUInt();
        }
    }
}
