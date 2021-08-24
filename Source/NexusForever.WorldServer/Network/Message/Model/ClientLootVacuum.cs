using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientLootVacuum)]
    public class ClientLootVacuum : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
