using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Fortunes
{
    [Message(GameMessageOpcode.ClientFortunesStart)]
    public class ClientFortunesStart : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // zero byte message
        }
    }
}
