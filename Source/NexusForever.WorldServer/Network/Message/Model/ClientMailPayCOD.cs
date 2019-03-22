using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientMailPayCOD, MessageDirection.Client)]
    public class ClientMailPayCOD : IReadable
    {
        public ulong MailId { get; private set; }
        public uint UnitId { get; private set; } // Mailbox Entity Guid

        public void Read(GamePacketReader reader)
        {
            MailId = reader.ReadULong();
            UnitId  = reader.ReadUInt();
        }
    }
}
