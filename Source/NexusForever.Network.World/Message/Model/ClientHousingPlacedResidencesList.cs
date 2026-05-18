using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingPlacedResidencesList)]
    public class ClientHousingPlacedResidencesList : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
