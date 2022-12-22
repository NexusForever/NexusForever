using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMailReturn)]
    public class ClientMailReturn : IReadable
    {
        public ulong MailId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            MailId = reader.ReadULong();
        }
    }
}
