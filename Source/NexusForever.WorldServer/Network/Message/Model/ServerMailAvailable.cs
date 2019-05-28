using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Mail.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerMailAvailable)]
    public class ServerMailAvailable : IWritable
    {
        public class Attachment : IWritable
        {
            public uint ItemId { get; set; } // 18
            public uint Amount { get; set; }
            public uint Unknown3 { get; set; }
            public ulong Unknown4 { get; set; }
            public uint Unknown5 { get; set; }
            public ulong Unknown6 { get; set; }
            public uint Unknown7 { get; set; } // 18
            public byte[] Unknown8 { get; set; } = new byte[20];
            public byte[] Unknown9 { get; set; } = new byte[32];

            public void Write(GamePacketWriter writer)
            {
                writer.Write(ItemId, 18u);
                writer.Write(Amount);
                writer.Write(Unknown3);
                writer.Write(Unknown4);
                writer.Write(Unknown5);
                writer.Write(Unknown6);
                writer.Write(Unknown7, 18u);

                for (uint i = 0u; i < Unknown8.Length; i++)
                    writer.Write(Unknown8[i]);

                for (uint i = 0u; i < Unknown9.Length; i++)
                    writer.Write(Unknown9[i]);
            }
        }

        public class Mail : IWritable
        {
            public ulong MailId { get; set; }
            public SenderType SenderType { get; set; }
            public ushort Unknown0 { get; set; }
            public string Subject { get; set; }
            public string Message { get; set; }
            public uint TextEntrySubject { get; set; }
            public uint TextEntryMessage { get; set; }
            public uint CreatureId { get; set; }
            public byte CurrencyGiftType { get; set; }
            public ulong CurrencyGiftAmount { get; set; }
            public ulong CostOnDeliveryAmount { get; set; }
            public float ExpiryTimeInDays { get; set; }
            public MailFlag Flags { get; set; }
            public TargetPlayerIdentity Sender { get; set; } = new TargetPlayerIdentity();
            public List<Attachment> Attachments { get; set; } = new List<Attachment>();

            public void Write(GamePacketWriter writer)
            {
                writer.Write(MailId);
                writer.Write(SenderType, 32);
                writer.Write(Unknown0, 14);
                writer.WriteStringWide(Subject);
                writer.WriteStringWide(Message);
                writer.Write(TextEntrySubject, 21);
                writer.Write(TextEntryMessage, 21);
                writer.Write(CreatureId, 18);
                writer.Write(CurrencyGiftType, 4);
                writer.Write(CurrencyGiftAmount);
                writer.Write(CostOnDeliveryAmount);
                writer.Write(ExpiryTimeInDays);
                writer.Write(Flags, 32);
                writer.Write(Sender.RealmId, 14);
                writer.Write(Sender.CharacterId);

                writer.Write(Attachments.Count);
                Attachments.ForEach(v => v.Write(writer));
            }
        }

        public bool NewMail { get; set; } = true;
        public List<Mail> MailList { get; set; } = new List<Mail>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(NewMail);

            writer.Write(MailList.Count);
            MailList.ForEach(v => v.Write(writer));
        }
    }
}
