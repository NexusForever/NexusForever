using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientBuybackItemFromVendor, MessageDirection.Client)]
    public class ClientBuybackItemFromVendor : IReadable
    {
        public uint BuybackPosition { get; private set; }

        public void Read(GamePacketReader reader)
        {
            BuybackPosition = reader.ReadByte();
        }
    }
}
