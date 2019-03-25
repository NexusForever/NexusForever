using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Mail.Static;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using MailModel = NexusForever.WorldServer.Database.Character.Model.CharacterMail;

namespace NexusForever.WorldServer.Game.Mail
{
    public class MailItem
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public ulong Id { get; }

        public ulong RecipientId {
            get => recipientId;
            private set
            {
                if (value == recipientId)
                    throw new ArgumentException("Receipient ID must be different than current receipient.");
                recipientId = value;
                saveMask |= MailSaveMask.RecipientChange;
            }
        }
        private ulong recipientId;

        public SenderType SenderType { get; }
        public ulong SenderId { get; private set; } = 0;

        public string Subject { get; private set; } = "";
        public string Message { get; private set; } = "";

        public uint TextEntrySubject { get; } = 0;
        public uint TextEntryMessage { get; } = 0;
        public uint CreatureId { get; } = 0;

        public CurrencyType CurrencyType { get; }
        public ulong CurrencyAmount { get; }
        public bool IsCashOnDelivery { get; }

        public bool HasPaidOrCollectedCurrency {
            get => hasPaidOrCollectedCurrency;
            private set
            {
                hasPaidOrCollectedCurrency = value;
                saveMask |= MailSaveMask.CurrencyChange;
            }
        }
        private bool hasPaidOrCollectedCurrency;

        public MailFlag Flags {
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

        private MailSaveMask saveMask;

        public bool IsPendingDelete => ((saveMask & MailSaveMask.Delete) != 0);

        private readonly List<MailAttachment> mailAttachments = new List<MailAttachment>();
        private readonly HashSet<MailAttachment> deletedAttachments = new HashSet<MailAttachment>();

        /// <summary>
        /// Create a new <see cref="MailItem"/> from an existing <see cref="MailModel"/>
        /// </summary>
        /// <param name="model"></param>
        public MailItem(MailModel model)
        {
            Id = model.Id;
            recipientId = model.RecipientId;
            SenderType = (SenderType)model.SenderType;
            SenderId = model.SenderId;
            Subject = model.Subject;
            Message = model.Message;
            CurrencyType = (CurrencyType)model.CurrencyType;
            CurrencyAmount = model.CurrencyAmount;
            IsCashOnDelivery = Convert.ToBoolean(model.IsCashOnDelivery);
            hasPaidOrCollectedCurrency = Convert.ToBoolean(model.HasPaidOrCollectedCurrency);
            flags = (MailFlag)model.Flags;
            DeliveryTime = (DeliveryTime)model.DeliveryTime;
            CreateTime = model.CreateTime;

            foreach (CharacterMailAttachment mailAttachment in model.CharacterMailAttachment)
                mailAttachments.Add(new MailAttachment(mailAttachment));

            saveMask = MailSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="MailItem"/> for a new sent mail
        /// </summary>
        /// <param name="newRecipientId"></param>
        /// <param name="sender"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="currencyAmount"></param>
        /// <param name="isCOD"></param>
        /// <param name="deliveryTime"></param>
        /// <param name="attachedItems"></param>
        public MailItem(ulong newRecipientId, Player sender, string subject, string message, ulong currencyAmount, bool isCOD, DeliveryTime deliveryTime)
        {
            Id = MailManager.NextMailId;
            recipientId = newRecipientId;
            SenderType = SenderType.Player;
            SenderId = sender.CharacterId;
            Subject = subject;
            Message = message;
            CurrencyType = CurrencyType.Credits;
            CurrencyAmount = currencyAmount;
            if (isCOD)
                IsCashOnDelivery = true;
            hasPaidOrCollectedCurrency = false;
            flags = MailFlag.None;
            DeliveryTime = deliveryTime;
            CreateTime = DateTime.Now;

            saveMask = MailSaveMask.Create;
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
            log.Info($"Save called on {Id}");

            if (saveMask != MailSaveMask.None)
            {
                if ((saveMask & MailSaveMask.Create) != 0)
                {
                    context.Add(new MailModel
                    {
                        Id = Id,
                        RecipientId = RecipientId,
                        SenderType = (byte)SenderType,
                        SenderId = SenderId,
                        Subject = Subject,
                        Message = Message,
                        TextEntrySubject = TextEntrySubject,
                        TextEntryMessage = TextEntryMessage,
                        CreatureId = CreatureId,
                        CurrencyType = Convert.ToByte(CurrencyType),
                        CurrencyAmount = CurrencyAmount,
                        IsCashOnDelivery = Convert.ToByte(IsCashOnDelivery),
                        HasPaidOrCollectedCurrency = Convert.ToByte(HasPaidOrCollectedCurrency),
                        Flags = (byte)Flags,
                        DeliveryTime = (byte)DeliveryTime,
                        CreateTime = CreateTime
                    });
                }
                else if ((saveMask & MailSaveMask.Delete) != 0)
                {
                    var model = new MailModel
                    {
                        Id = Id
                    };

                    context.Entry(model).State = EntityState.Deleted;
                }
                else
                {
                    var model = new MailModel
                    {
                        Id = Id
                    };

                    EntityEntry<MailModel> entity = context.Attach(model);
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
        }

        /// <summary>
        /// Returns the specific <see cref="MailAttachment"/> based on its index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MailAttachment GetAttachment(int index)
        {
            return mailAttachments[index];
        }

        /// <summary>
        /// Return all <see cref="MailAttachment"/> for the <see cref="MailItem"/>.
        /// </summary>
        public IEnumerable<MailAttachment> GetAttachments()
        {
            return mailAttachments;
        }

        /// <summary>
        /// Mark this <see cref="MailItem"/> as read by the player.
        /// </summary>
        public void MarkAsRead()
        {
            Flags |= MailFlag.IsRead;
        }

        /// <summary>
        /// Mark this <see cref="MailItem"/> as paid, or taken the currency attached
        /// </summary>
        public void PayOrTakeCash()
        {
            HasPaidOrCollectedCurrency = true;
            MarkAsNotReturnable();
        }

        /// <summary>
        /// Mark this <see cref="MailItem"/> as not returnable
        /// </summary>
        public void MarkAsNotReturnable()
        {
            Flags = MailFlag.NotReturnable;
        }

        /// <summary>
        /// Return this <see cref="MailItem"/> to sender
        /// </summary>
        public void ReturnMail()
        {
            RecipientId = SenderId;
            Subject = "Returned: " + Subject;
            MarkAsNotReturnable();
        }

        /// <summary>
        /// Returns whether this item is ready to be delivered based on <see cref="DeliveryTime"/>
        /// </summary>
        /// <returns></returns>
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
        /// <param name="mailAttachment"></param>
        public void AttachmentAdd(MailAttachment mailAttachment)
        {
            mailAttachments.Add(mailAttachment);
        }

        /// <summary>
        /// Remove a <see cref="MailAttachment"/> from this <see cref="MailItem"/>
        /// </summary>
        /// <param name="mailAttachment"></param>
        public void AttachmentDelete(MailAttachment mailAttachment, int index)
        {
            mailAttachment.EnqueueDelete();

            mailAttachments.RemoveAt(index);
            deletedAttachments.Add(mailAttachment);
        }
    }
}
