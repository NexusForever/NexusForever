using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Mail;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Mail;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Mail
{
    public class MailItem : IMailItem
    {
        /// <summary>
        /// Determines which fields need saving for <see cref="IMailItem"/> when being saved to the database.
        /// </summary>
        [Flags]
        public enum MailSaveMask
        {
            None                = 0x0000,
            Create              = 0x0001,
            Flags               = 0x0002,
            CurrencyChange      = 0x0004,
            RecipientChange     = 0x0008,
            Delete              = 0x0010
        }

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

        public bool PendingCreate => (saveMask & MailSaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="IMailItem"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & MailSaveMask.Delete) != 0;

        private MailSaveMask saveMask;

        private readonly List<IMailAttachment> mailAttachments = new();
        private readonly HashSet<IMailAttachment> deletedAttachments = new();

        /// <summary>
        /// Create a new <see cref="IMailItem"/> from an existing <see cref="CharacterMailModel"/>.
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
        /// Create a new <see cref="IMailItem"/> from supplied <see cref="IMailParameters"/>.
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
        /// Enqueue <see cref="IMailItem"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete(bool state)
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

            foreach (IMailAttachment mailAttachment in mailAttachments)
                mailAttachment.Save(context);

            foreach (IMailAttachment mailAttachment in deletedAttachments)
                mailAttachment.Save(context);

            deletedAttachments.Clear();
        }

        /// <summary>
        /// Returns the specific <see cref="IMailAttachment"/> based on its index.
        /// </summary>
        public IMailAttachment GetAttachment(uint index)
        {
            return index < mailAttachments.Count ? mailAttachments[(int)index] : null;
        }

        /// <summary>
        /// Mark this <see cref="IMailItem"/> as read by the player.
        /// </summary>
        public void MarkAsRead()
        {
            Flags |= MailFlag.IsRead;
        }

        /// <summary>
        /// Mark this <see cref="IMailItem"/> as paid, or taken the currency attached.
        /// </summary>
        public void PayOrTakeCash()
        {
            HasPaidOrCollectedCurrency = true;
            MarkAsNotReturnable();
        }

        /// <summary>
        /// Mark this <see cref="IMailItem"/> as not returnable.
        /// </summary>
        public void MarkAsNotReturnable()
        {
            Flags |= MailFlag.NotReturnable;
        }

        /// <summary>
        /// Return this <see cref="IMailItem"/> to sender.
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
        /// Add a <see cref="IMailAttachment"/> to this <see cref="IMailItem"/>.
        /// </summary>
        public void AttachmentAdd(IMailAttachment mailAttachment)
        {
            mailAttachments.Add(mailAttachment);
        }

        /// <summary>
        /// Remove a <see cref="IMailAttachment"/> from this <see cref="IMailItem"/>.
        /// </summary>
        public void AttachmentDelete(IMailAttachment mailAttachment, uint index)
        {
            mailAttachment.EnqueueDelete();

            mailAttachments.RemoveAt((int)index);
            deletedAttachments.Add(mailAttachment);
        }

        public ServerMailAvailable.Mail Build()
        {
            bool isPlayer = SenderType == SenderType.Player || SenderType == SenderType.GM;

            var serverMailItem = new ServerMailAvailable.Mail
            {
                MailId               = Id,
                SenderType           = SenderType,
                Subject              = Subject,
                Message              = Message,
                TextEntrySubject     = TextEntrySubject,
                TextEntryMessage     = TextEntryMessage,
                CreatureId           = !isPlayer ? CreatureId : 0,
                CurrencyGiftType     = 0,
                CurrencyGiftAmount   = !IsCashOnDelivery && !HasPaidOrCollectedCurrency ? CurrencyAmount : 0,
                CostOnDeliveryAmount = IsCashOnDelivery && !HasPaidOrCollectedCurrency ? CurrencyAmount : 0,
                ExpiryTimeInDays     = ExpiryTime,
                Flags                = Flags,
                Sender = new TargetPlayerIdentity
                {
                    RealmId     = isPlayer ? RealmContext.Instance.RealmId : (ushort)0,
                    CharacterId = isPlayer ? SenderId : 0ul
                },
            };

            foreach (IMailAttachment attachment in mailAttachments)
                serverMailItem.Attachments.Add(attachment.Build());

            return serverMailItem;
        }

        public IEnumerator<IMailAttachment> GetEnumerator()
        {
            return mailAttachments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
