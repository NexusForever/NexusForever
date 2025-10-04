using NexusForever.Game.Static.Mail;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Mail
{
    [Message(GameMessageOpcode.ServerMailAvailable)]
    public class ServerMailAvailable : IWritable
    {
        public class Attachment : IWritable
        {
            public uint Item2Id { get; set; }
            public uint StackCount { get; set; }
            public uint Charges { get; set; }
            public ulong ItemRandomCircuitData { get; set; }
            public uint ItemRandomGlyphData { get; set; }
            public ulong ThresholdData { get; set; }
            public uint Unknown7 { get; set; }
            public uint[] Item2IdMicrochip { get; set; } = new uint[5];
            public uint[] Item2IdGlyph { get; set; } = new uint[8];

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Item2Id, 18u);
                writer.Write(StackCount);
                writer.Write(Charges);
                writer.Write(ItemRandomCircuitData);
                writer.Write(ItemRandomGlyphData);
                writer.Write(ThresholdData);
                writer.Write(Unknown7, 18u);

                for (uint i = 0u; i < Item2IdMicrochip.Length; i++)
                    writer.Write(Item2IdMicrochip[i]);

                for (uint i = 0u; i < Item2IdGlyph.Length; i++)
                    writer.Write(Item2IdGlyph[i]);
            }
        }

        public class Mail : IWritable
        {
            public ulong MailId { get; set; }
            public SenderType SenderType { get; set; }
            public ContentType ContentType { get; set; } // Client uses this to format the mail content. More entries TBC
            public string Subject { get; set; }
            public string Message { get; set; }
            public uint SubjectStringId { get; set; } // LocalizedStringId. If set, Subject is ignored.
            public uint BodyStringId { get; set; } // LocalizedStringId. If set, Message is ignored.
            public uint FromCreatureId { get; set; }
            public byte CurrencySentType { get; set; }
            public ulong CurrencySentAmount { get; set; }
            public ulong CostOnDeliveryAmount { get; set; }
            public float ExpiryTimeInDays { get; set; }
            public MailFlag Flags { get; set; }
            public Identity Sender { get; set; } = new();
            public List<Attachment> Attachments { get; set; } = new();

            public void Write(GamePacketWriter writer)
            {
                writer.Write(MailId);
                writer.Write(SenderType, 32);
                writer.Write(ContentType, 14);
                writer.WriteStringWide(Subject);
                writer.WriteStringWide(Message);
                writer.Write(SubjectStringId, 21);
                writer.Write(BodyStringId, 21);
                writer.Write(FromCreatureId, 18);
                writer.Write(CurrencySentType, 4);
                writer.Write(CurrencySentAmount);
                writer.Write(CostOnDeliveryAmount);
                writer.Write(ExpiryTimeInDays);
                writer.Write(Flags, 32);
                Sender.Write(writer);

                writer.Write(Attachments.Count);
                Attachments.ForEach(v => v.Write(writer));
            }
        }

        public bool NewMail { get; set; } = true;
        public List<Mail> MailList { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(NewMail);

            writer.Write(MailList.Count);
            MailList.ForEach(v => v.Write(writer));
        }
    }
}
