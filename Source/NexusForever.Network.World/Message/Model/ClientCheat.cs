using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCheat)]
    public class ClientCheat : IReadable
    {
        public string Message { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Message = reader.ReadWideString();
        }
    }
}