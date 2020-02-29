using System;
using System.Collections.Generic;

namespace NexusForever.Database.Character.Model
{
    public class CharacterMailModel
    {
        public ulong Id { get; set; }
        public ulong RecipientId { get; set; }
        public byte SenderType { get; set; }
        public ulong SenderId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public uint TextEntrySubject { get; set; }
        public uint TextEntryMessage { get; set; }
        public uint CreatureId { get; set; }
        public byte CurrencyType { get; set; }
        public ulong CurrencyAmount { get; set; }
        public byte IsCashOnDelivery { get; set; }
        public byte HasPaidOrCollectedCurrency { get; set; }
        public byte Flags { get; set; }
        public byte DeliveryTime { get; set; }
        public DateTime CreateTime { get; set; }

        public CharacterModel Recipient { get; set; }
        public ICollection<CharacterMailAttachmentModel> Attachment { get; set; } = new HashSet<CharacterMailAttachmentModel>();
    }
}
