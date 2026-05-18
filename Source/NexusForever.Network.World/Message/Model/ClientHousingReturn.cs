using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingReturn)]
    public class ClientHousingReturn : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
