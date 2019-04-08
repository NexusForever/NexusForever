using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientCheat)]
    public class ClientCheat : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            Message = reader.ReadWideString();
        }

        public string Message { get; private set; }
    }
}