using NexusForever.Database.Character;
using NexusForever.Game.Static.Contact;

namespace NexusForever.Game.Abstract.Contact
{
    public interface IContact : IDatabaseCharacter
    {
        ulong ContactId { get; }
        ulong Id { get; }
        string InviteMessage { get; set; }
        bool IsPendingAcceptance { get; }
        bool IsPendingCreate { get; }
        bool IsPendingDelete { get; }
        ulong OwnerId { get; }
        string PrivateNote { get; set; }
        DateTime RequestTime { get; set; }
        ContactType Type { get; set; }

        /// <summary>
        /// Used to mark this <see cref="Contact"/> as an accepted contact by the recipient.
        /// </summary>
        void AcceptRequest();

        /// <summary>
        /// USed to mark this <see cref="Contact"/> as a declined contact, or deleted request.
        /// </summary>
        void DeclineRequest();

        /// <summary>
        /// Enqueue <see cref="Contact"/> to be deleted from the database.
        /// </summary>
        void EnqueueDelete();

        /// <summary>
        /// Used to mark this <see cref="Contact"/> as pending acceptance
        /// </summary>
        void MakePendingAcceptance();
    }
}
