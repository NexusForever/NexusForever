using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPathScientistDismissScanbot2)]
    public class ClientPathScientistDismissScanbot2 : IReadable
    {
        // This is a zero byte message.
        public void Read(GamePacketReader reader)
        {
        }
    }
}
