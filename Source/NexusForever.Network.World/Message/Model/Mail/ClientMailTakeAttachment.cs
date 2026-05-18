using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Mail
{
    [Message(GameMessageOpcode.ClientMailTakeAttachment)]
    public class ClientMailTakeAttachment : IReadable
    {
        public ulong MailId { get; private set; }
        public uint Index { get; private set; }
        public uint MailboxUnitId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            MailId = reader.ReadULong();
            Index = reader.ReadUInt();
            MailboxUnitId = reader.ReadUInt();
        }
    }
}
