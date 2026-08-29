using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Mail
{
    [Message(GameMessageOpcode.ClientMailTakeAllFromSelection)]
    public class ClientMailTakeAllFromSelection : IReadable
    {
        public List<ulong> MailList { get; } = [];
        public uint MailboxUnitId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUInt();
            for (int i = 0; i < count; i++)
            {
                MailList.Add(reader.ReadULong());
            }

            MailboxUnitId = reader.ReadUInt();
        }
    }
}
