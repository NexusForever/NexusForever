using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Mail.Static;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientMailSend)]
    public class ClientMailSend : IReadable
    {
        public string Name { get; private set; }
        public string Realm { get; private set; }
        public string Subject { get; private set; }
        public string Message { get; private set; }
        public ulong CreditsSent { get; private set; }
        public ulong CreditsRequested { get; private set; }
        public DeliveryTime DeliveryTime { get; private set; } // 0 Instant, 1 Hour, 2, Day
        public uint UnitId { get; private set; } // Mailbox Entity Guid
        public List<ulong> Items { get; set; } = new List<ulong>();

        public void Read(GamePacketReader reader)
        {
            Name             = reader.ReadWideString();
            Realm            = reader.ReadWideString();
            Subject          = reader.ReadWideString();
            Message          = reader.ReadWideString();
            CreditsSent      = reader.ReadULong();
            CreditsRequested = reader.ReadULong();
            DeliveryTime     = reader.ReadEnum<DeliveryTime>(2u);
            UnitId           = reader.ReadUInt();

            for (uint i = 0; i < 10; i++)
                Items.Add(reader.ReadULong());
        }
    }
}
