using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupReadyCheckRequest)]
    public class ClientGroupReadyCheckRequest : IReadable
    {
        public ulong GroupId { get; private set; }
        public string Message { get; private set; }

        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
            Message = reader.ReadWideString();
        }
    }
}
