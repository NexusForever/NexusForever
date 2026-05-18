using NexusForever.Game.Static.Mail;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Mail
{
    [Message(GameMessageOpcode.ClientMailSend)]
    public class ClientMailSend : IReadable
    {
        public string Name { get; private set; }
        public string Realm { get; private set; }
        public string Subject { get; private set; }
        public string Message { get; private set; }
        public ulong CreditsSent { get; private set; }
        public ulong CashOnDeliveryAmount { get; private set; }
        public DeliverySpeed DeliverySpeed { get; private set; }
        public uint MailboxUnitId { get; private set; }
        public List<ulong> Items { get; private set; } = [];

        public void Read(GamePacketReader reader)
        {
            Name = reader.ReadWideString();
            Realm = reader.ReadWideString();
            Subject = reader.ReadWideString();
            Message = reader.ReadWideString();
            CreditsSent = reader.ReadULong();
            CashOnDeliveryAmount = reader.ReadULong();
            DeliverySpeed = reader.ReadEnum<DeliverySpeed>(2u);
            MailboxUnitId = reader.ReadUInt();

            for (uint i = 0; i < 10; i++)
                Items.Add(reader.ReadULong());
        }
    }
}
