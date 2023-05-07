using NexusForever.Game.Abstract.Contact;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Contact;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Contact
{
    public interface IGlobalContactManager : IUpdate
    {
        /// <summary>
        /// Id to be assigned to the next created contact.
        /// </summary>
        ulong NextContactId { get; }

        /// <summary>
        /// Initialise the <see cref="IGlobalContactManager"/>.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Shutdown <see cref="IGlobalContactManager"/> and any related resources.
        /// </summary>
        /// <remarks>
        /// This will force save all pending contacts.
        /// </remarks>
        void Shutdown();

        void AcceptRequest(IContact contact);

        /// <summary>
        /// Checks to see if the target character can become a contact
        /// </summary>
        bool CanBeContact(IPlayer player, ulong recipientId, ContactType type, Dictionary<ulong, IContact> playerContacts);

        /// <summary>
        /// Checks to see if the target character can become a contact, and returns a <see cref="ContactResult"/> appropriately
        /// </summary>
        ContactResult CanBeContactResult(IPlayer player, ulong receipientId, ContactType type, Dictionary<ulong, IContact> playerContacts);

        /// <summary>
        /// Declines the pending <see cref="IContact"/> request.
        /// </summary>
        void DeclineRequest(IContact contact);

        /// <summary>
        /// Maximum amount of time a contact request can sit for before expiring
        /// </summary>
        float GetMaxRequestDurationInDays();

        /// <summary>
        /// Get all <see cref="IContact"/> request responses which have been queued for save
        /// </summary>
        IEnumerable<IContact> GetQueuedRequests(ulong ownerId);

        /// <summary>
        /// Notifies online <see cref="IContact"/> owners, that the player has logged in or out.
        /// </summary>
        void NotifySubscriber(ulong subscriberId, ulong contactCharacterId, bool loggingOut = false);

        /// <summary>
        /// Notifies online <see cref="IContact"/> owners, that the player has logged in or out.
        /// </summary>
        void NotifySubscribers(ulong characterId, bool loggingOut = false);

        /// <summary>
        /// Remove subscriber with the provided Character ID from all previous subscriptions
        /// </summary>
        /// <param name="characterId">Character ID being removed</param>
        void RemoveSubscriber(ulong characterId);

        /// <summary>
        /// Attempt to send a <see cref="IContact"/> request to it's target Player
        /// </summary>
        /// <param name="contactRequest">The Contact request you wish to send</param>
        void SendRequestToPlayer(IContact contactRequest);

        /// <summary>
        /// Subscribe player with the provided Character ID to notifications if & when users in the Character ID List come online
        /// </summary>
        void SubscribeTo(ulong characterId, IEnumerable<ulong> characterIdList);

        /// <summary>
        /// Remove a pending <see cref="IContact"/> request from an online player
        /// </summary>
        /// <param name="contactRequest">Contact request to remove</param>
        void TryRemoveRequestFromOnlineUser(IContact contactRequest);

        /// <summary>
        /// Unsubscribe player with the provided Character ID to notifications if & when users in the Character ID List come online
        /// </summary>
        void UnsubscribeFrom(ulong characterId, List<ulong> characterIdList);
    }
}