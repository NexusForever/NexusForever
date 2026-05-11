using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Fortune
{
    [Message(GameMessageOpcode.ClientFortuneStart)]
    public class ClientFortuneStart : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // zero byte message
        }
    }
}
