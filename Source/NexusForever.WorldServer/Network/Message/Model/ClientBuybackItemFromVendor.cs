using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
