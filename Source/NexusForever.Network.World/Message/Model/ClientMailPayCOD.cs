using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMailPayCod)]
    public class ClientMailPayCod : IReadable
    {
        public ulong MailId { get; private set; }
        public uint UnitId { get; private set; } // Mailbox Entity Guid

        public void Read(GamePacketReader reader)
        {
            MailId = reader.ReadULong();
            UnitId = reader.ReadUInt();
        }
    }
}
