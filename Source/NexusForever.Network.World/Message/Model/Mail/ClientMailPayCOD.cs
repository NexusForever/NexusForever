using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Mail
{
    [Message(GameMessageOpcode.ClientMailPayCod)]
    public class ClientMailPayCod : IReadable
    {
        public ulong MailId { get; private set; }
        public uint MailboxUnitId { get; private set; } // Must be interacting with a mailbox to pay COD

        public void Read(GamePacketReader reader)
        {
            MailId = reader.ReadULong();
            MailboxUnitId = reader.ReadUInt();
        }
    }
}
