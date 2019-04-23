using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingRandomResidenceList)]
    public class ClientHousingRandomResidenceList : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
