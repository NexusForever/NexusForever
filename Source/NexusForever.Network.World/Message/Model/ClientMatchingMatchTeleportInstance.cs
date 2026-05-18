using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMatchingTransferIntoMatch)]
    public class ClientMatchingTransferIntoMatch : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // deliberately empty
        }
    }
}
