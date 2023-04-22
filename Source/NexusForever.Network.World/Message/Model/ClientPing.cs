using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPing)]
    public class ClientPing : IReadable 
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
