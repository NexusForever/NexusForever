using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Mail
{
    [Message(GameMessageOpcode.ClientMailTakeCash)]
    public class ClientMailTakeCash : IReadable
    {
        public ulong MailId { get; private set; }
        public uint MailboxUnitId { get; private set; } // Mailbox being interacted with

        public void Read(GamePacketReader reader)
        {
            MailId = reader.ReadULong();
            MailboxUnitId = reader.ReadUInt();
        }
    }
}
