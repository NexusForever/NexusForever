using NexusForever.Database.Character;
using NexusForever.Game.Abstract.Contact;
using NexusForever.Game.Static.Contact;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IContactManager : IDatabaseCharacter
    {
        /// <summary>
        /// Accept a <see cref="Contact.Contact"/> request
        /// </summary>
        /// <param name="id">ID of the accepted request</param>
        /// <param name="returnRequest">True if the player is making the requester a friend as well</param>
        void AcceptFriendRequest(ulong id, bool returnRequest = false);

        /// <summary>
        /// Called by <see cref="IGlobalContactManager"/> when a request is accepted
        /// </summary>
        void ContactRequestAccepted(ulong contactId);

        /// <summary>
        /// Called by <see cref="IGlobalContactManager"/> when a contact request is made
        /// </summary>
        void ContactRequestCreate(IContact contact);

        /// <summary>
        /// Called by <see cref="IGlobalContactManager"/> when a request is accepted
        /// </summary>
        void ContactRequestDeclined(ulong contactId);

        /// <summary>
        /// Create a <see cref="Contact"/> request with another character
        /// </summary>
        /// <param name="recipientId">Character ID of the requested character</param>
        /// <param name="message">Message to send to the recipient</param>
        void CreateFriendRequest(ulong recipientId, string message);

        /// <summary>
        /// Create an ignored <see cref="IContact"/>
        /// </summary>
        /// <param name="recipientId">Character ID of the player to ignore</param>
        void CreateIgnored(ulong recipientId);

        /// <summary>
        /// Create a rival <see cref="IContact"/>
        /// </summary>
        /// <param name="recipientId">Character ID of the player to make a rival of</param>
        void CreateRival(ulong recipientId);

        /// <summary>
        /// Decline a <see cref="IContact"/> request
        /// </summary>
        /// <param name="contactId">Contact.ID of the declined request</param>
        /// <param name="addIgnore">Flag to create an ignore entry for this user</param>
        void DeclineFriendRequest(ulong contactId, bool addIgnore = false);

        /// <summary>
        /// Delete <see cref="IContact"/> from a player's contacts
        /// </summary>
        /// <param name="contactToDelete">Contact to be deleted</param>
        /// <param name="requestedTypeToDelete">Contact type to be deleted</param>
        void DeleteContact(IContact contactToDelete, ContactType requestedTypeToDelete);

        /// <summary>
        /// Delete <see cref="IContact"/> from a player's contacts
        /// </summary>
        /// <param name="playerIdentity"><see cref="TargetPlayerIdentity"/> of the player to delete</param>
        /// <param name="type">Type to confirm deletion with</param>f
        void DeleteContact(TargetPlayerIdentity playerIdentity, ContactType type);

        /// <summary>
        /// Create a <see cref="IContact"/> and force it to an accepted state
        /// </summary>
        /// <param name="recipientId">Character ID that will be the contact</param>
        /// <param name="type">Type of contact</param>
        void ForceCreateContact(ulong recipientId, ContactType type);

        /// <summary>
        /// Returns whether or not the supplied Character ID is ignored.
        /// </summary>
        /// <param name="characterId">Character ID to check</param>
        bool IsIgnored(ulong characterId);

        /// <summary>
        /// Called by an <see cref="IPlayer"/> when logging in
        /// </summary>
        void OnLogin();

        /// <summary>
        /// Called by an <see cref="IPlayer"/> when logging out
        /// </summary>
        void OnLogout();

        /// <summary>
        /// Send a Contacts Result packet to the player
        /// </summary>
        /// <param name="result">Result code to be used</param>
        void SendContactsResult(ContactResult result);

        /// <summary>
        /// Set a private note associated with a <see cref="IContact"/>
        /// </summary>
        /// <param name="contactToModify"></param>
        /// <param name="note"></param>
        void SetPrivateNote(IContact contactToModify, string Note);

        /// <summary>
        /// Set a private note associated with a <see cref="Contact"/>
        /// </summary>
        /// <param name="playerIdentity">CharacterIdentity of the player's note to update</param>
        /// <param name="note">Note to set</param>
        void SetPrivateNote(TargetPlayerIdentity playerIdentity, string note);
    }
}