using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections;
using System.Collections.Generic;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Mail.Static;

namespace NexusForever.WorldServer.Game.Mail
{
    public class MailItem : IEnumerable<MailAttachment>
    {
        public ulong Id { get; }

        public ulong RecipientId
        {
            get => recipientId;
            private set
            {
                if (value == recipientId)
                    throw new ArgumentException("Recipient ID must be different than current recipient.");
                recipientId = value;
                saveMask |= MailSaveMask.RecipientChange;
            }
        }
        private ulong recipientId;

        public SenderType SenderType { get; }
        public ulong SenderId { get; }
        public uint CreatureId { get; }

        public string Subject { get; private set; } = "";
        public string Message { get; } = "";
        public uint TextEntrySubject { get; }
        public uint TextEntryMessage { get; }

        public CurrencyType CurrencyType { get; }
        public ulong CurrencyAmount { get; }
        public bool IsCashOnDelivery { get; }

        public bool HasPaidOrCollectedCurrency
        {
            get => hasPaidOrCollectedCurrency;
            private set
            {
                hasPaidOrCollectedCurrency = value;
                saveMask |= MailSaveMask.CurrencyChange;
            }
        }
        private bool hasPaidOrCollectedCurrency;

        public MailFlag Flags
        {
            get => flags;
            private set
            {
                flags = value;
                saveMask |= MailSaveMask.Flags;
            }
        }
        private MailFlag flags;

        public DeliveryTime DeliveryTime { get; }
        public DateTime CreateTime { get; }
        public float ExpiryTime => 30f; // TODO: Make this configurable

        /// <summary>
        /// Returns if <see cref="MailItem"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & MailSaveMask.Delete) != 0;

        private MailSaveMask saveMask;

        private readonly List<MailAttachment> mailAttachments = new List<MailAttachment>();
        private readonly HashSet<MailAttachment> deletedAttachments = new HashSet<MailAttachment>();

        /// <summary>
        /// Create a new <see cref="MailItem"/> from an existing <see cref="CharacterMailModel"/>.
        /// </summary>
        public MailItem(CharacterMailModel model)
        {
            Id                         = model.Id;
            recipientId                = model.RecipientId;
            SenderType                 = (SenderType)model.SenderType;
            SenderId                   = model.SenderId;
            Subject                    = model.Subject;
            Message                    = model.Message;
            TextEntrySubject           = model.TextEntrySubject;
            TextEntryMessage           = model.TextEntryMessage;
            CreatureId                 = model.CreatureId;
            CurrencyType               = (CurrencyType)model.CurrencyType;
            CurrencyAmount             = model.CurrencyAmount;
            IsCashOnDelivery           = Convert.ToBoolean(model.IsCashOnDelivery);
            hasPaidOrCollectedCurrency = Convert.ToBoolean(model.HasPaidOrCollectedCurrency);
            flags                      = (MailFlag)model.Flags;
            DeliveryTime               = (DeliveryTime)model.DeliveryTime;
            CreateTime                 = model.CreateTime;

            foreach (CharacterMailAttachmentModel mailAttachment in model.Attachment)
                mailAttachments.Add(new MailAttachment(mailAttachment));

            saveMask = MailSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="MailItem"/> from supplied <see cref="MailParameters"/>.
        /// </summary>
        public MailItem(MailParameters parameters)
        {
            Id          = AssetManager.Instance.NextMailId;
            recipientId = parameters.RecipientCharacterId;
            SenderType  = parameters.MessageType;

            if (SenderType == SenderType.Player || SenderType == SenderType.GM)
                SenderId = parameters.SenderCharacterId;
            else
                CreatureId = parameters.CreatureId;

            if (parameters.SubjectStringId != 0u)
                TextEntrySubject = parameters.SubjectStringId;
            else
                Subject = parameters.Subject;

            if (parameters.BodyStringId != 0u)
                TextEntryMessage = parameters.BodyStringId;
            else
                Message = parameters.Body;

            CurrencyType = CurrencyType.Credits;

            if (parameters.CodAmount > 0ul)
            {
                CurrencyAmount = parameters.CodAmount;
                IsCashOnDelivery = true;
            }
            else
                CurrencyAmount = parameters.MoneyToGive;

            DeliveryTime = parameters.DeliveryTime;
            CreateTime   = DateTime.Now;

            saveMask     = MailSaveMask.Create;
        }

        /// <summary>
        /// Enqueue <see cref="MailItem"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete()
        {
            saveMask = MailSaveMask.Delete;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask != MailSaveMask.None)
            {
                if ((saveMask & MailSaveMask.Create) != 0)
                {
                    context.Add(new CharacterMailModel
                    {
                        Id                         = Id,
                        RecipientId                = RecipientId,
                        SenderType                 = (byte)SenderType,
                        SenderId                   = SenderId,
                        Subject                    = Subject,
                        Message                    = Message,
                        TextEntrySubject           = TextEntrySubject,
                        TextEntryMessage           = TextEntryMessage,
                        CreatureId                 = CreatureId,
                        CurrencyType               = (byte)CurrencyType,
                        CurrencyAmount             = CurrencyAmount,
                        IsCashOnDelivery           = Convert.ToByte(IsCashOnDelivery),
                        HasPaidOrCollectedCurrency = Convert.ToByte(HasPaidOrCollectedCurrency),
                        Flags                      = (byte)Flags,
                        DeliveryTime               = (byte)DeliveryTime,
                        CreateTime                 = CreateTime
                    });
                }
                else if ((saveMask & MailSaveMask.Delete) != 0)
                {
                    var model = new CharacterMailModel
                    {
                        Id = Id
                    };

                    context.Entry(model).State = EntityState.Deleted;
                }
                else
                {
                    var model = new CharacterMailModel
                    {
                        Id = Id
                    };

                    EntityEntry<CharacterMailModel> entity = context.Attach(model);
                    if ((saveMask & MailSaveMask.Flags) != 0)
                    {
                        model.Flags = Convert.ToByte(Flags);
                        entity.Property(p => p.Flags).IsModified = true;
                    }

                    if ((saveMask & MailSaveMask.CurrencyChange) != 0)
                    {
                        model.HasPaidOrCollectedCurrency = Convert.ToByte(HasPaidOrCollectedCurrency);
                        entity.Property(p => p.HasPaidOrCollectedCurrency).IsModified = true;
                    }

                    if ((saveMask & MailSaveMask.RecipientChange) != 0)
                    {
                        model.RecipientId = RecipientId;
                        entity.Property(p => p.RecipientId).IsModified = true;

                        model.Subject = Subject;
                        entity.Property(p => p.Subject).IsModified = true;
                    }
                }

                saveMask = MailSaveMask.None;
            }

            foreach (MailAttachment mailAttachment in mailAttachments)
                mailAttachment.Save(context);

            foreach (MailAttachment mailAttachment in deletedAttachments)
                mailAttachment.Save(context);

            deletedAttachments.Clear();
        }

        /// <summary>
        /// Returns the specific <see cref="MailAttachment"/> based on its index.
        /// </summary>
        public MailAttachment GetAttachment(uint index)
        {
            return index < mailAttachments.Count ? mailAttachments[(int)index] : null;
        }

        /// <summary>
        /// Mark this <see cref="MailItem"/> as read by the player.
        /// </summary>
        public void MarkAsRead()
        {
            Flags |= MailFlag.IsRead;
        }

        /// <summary>
        /// Mark this <see cref="MailItem"/> as paid, or taken the currency attached.
        /// </summary>
        public void PayOrTakeCash()
        {
            HasPaidOrCollectedCurrency = true;
            MarkAsNotReturnable();
        }

        /// <summary>
        /// Mark this <see cref="MailItem"/> as not returnable.
        /// </summary>
        public void MarkAsNotReturnable()
        {
            Flags |= MailFlag.NotReturnable;
        }

        /// <summary>
        /// Return this <see cref="MailItem"/> to sender.
        /// </summary>
        public void ReturnMail()
        {
            RecipientId = SenderId;
            Subject = $"Returned: {Subject}";
            MarkAsNotReturnable();
        }

        /// <summary>
        /// Returns whether this item is ready to be delivered based on <see cref="DeliveryTime"/>.
        /// </summary>
        public bool IsReadyToDeliver()
        {
            if (DeliveryTime == DeliveryTime.Instant)
                return true;

            if (DeliveryTime == DeliveryTime.Hour)
                return DateTime.Now
                    .Subtract(CreateTime)
                    .TotalHours > 1;

            if (DeliveryTime == DeliveryTime.Day)
                return DateTime.Now
                    .Subtract(CreateTime)
                    .TotalDays > 1;

            return false;
        }

        /// <summary>
        /// Add a <see cref="MailAttachment"/> to this <see cref="MailItem"/>.
        /// </summary>
        public void AttachmentAdd(MailAttachment mailAttachment)
        {
            mailAttachments.Add(mailAttachment);
        }

        /// <summary>
        /// Remove a <see cref="MailAttachment"/> from this <see cref="MailItem"/>.
        /// </summary>
        public void AttachmentDelete(MailAttachment mailAttachment, uint index)
        {
            mailAttachment.EnqueueDelete();

            mailAttachments.RemoveAt((int)index);
            deletedAttachments.Add(mailAttachment);
        }

        public IEnumerator<MailAttachment> GetEnumerator()
        {
            return mailAttachments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
