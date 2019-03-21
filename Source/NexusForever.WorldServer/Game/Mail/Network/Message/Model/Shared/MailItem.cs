using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Mail.Static;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Mail.Network.Message.Model.Shared
{
    public class MailItem : IWritable
    {
       public ulong MailId { get; set; }
       public SenderType SenderType { get; set; }
       public ushort Unknown0 { get; set; } = 0; // 14
       public string Subject { get; set; }
       public string Message { get; set; }
       public uint TextEntrySubject { get; set; }
       public uint TextEntryMessage { get; set; }
       public uint CreatureId { get; set; }
       public byte CurrencyGiftType { get; set; }
       public ulong CurrencyGiftAmount { get; set; }
       public ulong CostOnDeliveryAmount { get; set; }
       public float ExpiryTimeInDays { get; set; }
       public MailFlag Flags { get; set; } = MailFlag.None;
       public ushort SenderRealm { get; set; } // Replace with CharacterIdentity from Contacts branch
       public ulong SenderCharacterId { get; set; } // Replace with CharacterIdentity from Contacts branch
       public List<MailAttachment> Attachments { get; set; } = new List<MailAttachment>();

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
           writer.Write(SenderRealm, 14);
           writer.Write(SenderCharacterId);

           writer.Write(Attachments.Count);
           Attachments.ForEach(v => v.Write(writer));
       }
    }
}