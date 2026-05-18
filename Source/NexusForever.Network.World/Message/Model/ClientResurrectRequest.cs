using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientResurrectRequest)]
    public class ClientResurrectRequest : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
