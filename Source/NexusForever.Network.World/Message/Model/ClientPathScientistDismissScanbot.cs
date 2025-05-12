using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPathScientistDismissScanbot1)]
    public class ClientPathScientistDismissScanbot1 : IReadable
    {
        // This is a zero byte message.
        public void Read(GamePacketReader reader)
        {
        }
    }
}
