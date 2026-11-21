using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingRequestRandomResidenceList)]
    public class ClientHousingRequestRandomResidenceList : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // Zero byte message
        }
    }
}
