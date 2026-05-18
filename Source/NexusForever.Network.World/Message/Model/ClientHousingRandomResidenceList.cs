using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingRandomResidenceList)]
    public class ClientHousingRandomResidenceList : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
