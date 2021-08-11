using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingPlacedResidencesList)]
    public class ClientHousingPlacedResidencesList : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
