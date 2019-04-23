using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientLogoutRequest)]
    public class ClientLogoutRequest : IReadable
    {
        public bool Initiated { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Initiated = reader.ReadBit();
        }
    }
}
