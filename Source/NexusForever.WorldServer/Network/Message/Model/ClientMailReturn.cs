using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
