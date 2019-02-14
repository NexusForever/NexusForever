using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Contact;
using NexusForever.Game.Static.Contact;

namespace NexusForever.Game.Contact
{
    public class Contact : IDatabaseCharacter, IContact
    {
        public ulong OwnerId { get; }
        public ulong Id { get; }
        public ulong ContactId { get; }

        public string InviteMessage
        {
            get => inviteMessage;
            set
            {
                inviteMessage = value;
                saveMask |= ContactSaveMask.Modify;
            }
        }
        private string inviteMessage { get; set; }

        public string PrivateNote
        {
            get => privateNote;
            set
            {
                privateNote = value;
                saveMask |= ContactSaveMask.Modify;
            }
        }
        private string privateNote { get; set; }

        public ContactType Type
        {
            get => type;
            set
            {
                if (type != value)
                    type = value;
                saveMask |= ContactSaveMask.Modify;
            }
        }
        private ContactType type { get; set; }

        public DateTime RequestTime
        {
            get => requestTime;
            set
            {
                requestTime = value;
                saveMask |= ContactSaveMask.Modify;
            }
        }
        private DateTime requestTime { get; set; }

        public bool IsPendingAcceptance => !Accepted;
        public bool IsPendingCreate => ((saveMask & ContactSaveMask.Create) != 0);
        public bool IsPendingDelete => ((saveMask & ContactSaveMask.Delete) != 0);

        private bool Accepted { get; set; }

        private ContactSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="Contact"/> from an existing <see cref="ContactsEntry"/> database model.
        /// </summary>
        public Contact(CharacterContactModel model)
        {
            Id = model.Id;
            OwnerId = model.OwnerId;
            ContactId = model.ContactId;
            InviteMessage = model.InviteMessage;
            PrivateNote = model.PrivateNote;
            Accepted = Convert.ToBoolean(model.Accepted);
            Type = (ContactType)model.Type;
            RequestTime = model.RequestTime;

            saveMask = ContactSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="Contact"/> from supplied item id.
        /// </summary>
        public Contact(ulong id, ulong ownerId, ulong contactId, string inviteMessage, ContactType type, bool isContactRequest = false)
        {
            Id = id;
            OwnerId = ownerId;
            ContactId = contactId;
            InviteMessage = inviteMessage;
            PrivateNote = "";
            Type = type;
            Accepted = !isContactRequest;
            RequestTime = DateTime.UtcNow;

            saveMask = ContactSaveMask.Create;
        }

        /// <summary>
        /// Used to mark this <see cref="Contact"/> as an accepted contact by the recipient.
        /// </summary>
        public void AcceptRequest()
        {
            Accepted = true;

            saveMask |= ContactSaveMask.Modify;
        }

        /// <summary>
        /// USed to mark this <see cref="Contact"/> as a declined contact, or deleted request.
        /// </summary>
        public void DeclineRequest()
        {
            if (Type == ContactType.Friend)
                EnqueueDelete();
            else
            {
                Accepted = true;
                saveMask |= ContactSaveMask.Modify;
            }
        }

        /// <summary>
        /// Used to mark this <see cref="Contact"/> as pending acceptance
        /// </summary>
        public void MakePendingAcceptance()
        {
            Accepted = false;

            saveMask |= ContactSaveMask.Modify;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Save(CharacterContext context)
        {
            if (saveMask == ContactSaveMask.None)
                return;

            if ((saveMask & ContactSaveMask.Create) != 0)
            {
                context.Add(new CharacterContactModel
                {
                    Id = Id,
                    OwnerId = OwnerId,
                    ContactId = ContactId,
                    PrivateNote = PrivateNote,
                    Type = (byte)Type,
                    Accepted = Convert.ToByte(Accepted),
                    RequestTime = RequestTime
                });
            }
            else if ((saveMask & ContactSaveMask.Delete) != 0)
            {
                var model = new CharacterContactModel
                {
                    Id = Id,
                    OwnerId = OwnerId,
                    ContactId = ContactId
                };

                context.Entry(model).State = EntityState.Deleted;
            }
            else
            {
                var model = new CharacterContactModel
                {
                    Id = Id,
                    OwnerId = OwnerId,
                    ContactId = ContactId,
                    RequestTime = RequestTime
                };

                if ((saveMask & ContactSaveMask.Modify) != 0)
                {
                    model.Accepted = Convert.ToByte(Accepted);
                    model.PrivateNote = PrivateNote;
                    model.Type = (byte)Type;
                    model.InviteMessage = InviteMessage;
                    model.RequestTime = RequestTime;

                    context.Entry(model).State = EntityState.Modified;
                }
            }

            saveMask = ContactSaveMask.None;
        }

        /// <summary>
        /// Enqueue <see cref="Contact"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete()
        {
            saveMask = ContactSaveMask.Delete;
        }
    }
}
