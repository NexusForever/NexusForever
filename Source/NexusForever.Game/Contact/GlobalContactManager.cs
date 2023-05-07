using NexusForever.WorldServer.Network.Message.Model;
using System.Collections.Concurrent;
using NexusForever.Shared;
using NexusForever.Shared.Game;
using NexusForever.Shared.Configuration;
using NexusForever.Game.Configuration.Model;
using NexusForever.Database.Character;
using NexusForever.Database;
using NexusForever.Game.Entity;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Game.Abstract.Contact;
using NexusForever.Network.World.Message.Model;
using NLog;

namespace NexusForever.Game.Contact
{
    public partial class GlobalContactManager : Singleton<GlobalContactManager>, IUpdate, IGlobalContactManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Maximum amount of time a contact request can sit for before expiring
        /// </summary>
        public float GetMaxRequestDurationInDays() => SharedConfiguration.Instance.Get<ContactConfig>()?.MaxRequestDuration ?? 7f; // ConfigurationManager<WorldServerConfiguration>.Instance.Config.Contacts.MaxRequestDuration ?? 7f;

        /// <summary>
        /// Id to be assigned to the next created contact.
        /// </summary>
        public ulong NextContactId => nextContactId++;
        private ulong nextContactId;

        private uint GetMaxFriends() => SharedConfiguration.Instance.Get<ContactConfig>()?.MaxFriends ?? 100u;
        private uint GetMaxRivals() => SharedConfiguration.Instance.Get<ContactConfig>()?.MaxRivals ?? 100u;
        private uint GetMaxIgnored() => SharedConfiguration.Instance.Get<ContactConfig>()?.MaxIgnored ?? 100u;

        /// <summary>
        /// Minimum Id for the contact entry; Required to prevent the Client from marking the contact as Temporary
        /// </summary>
        private readonly ulong temporaryMod = 281474976710656;

        private readonly ConcurrentDictionary</*guid*/ ulong, IContact> contactsToSave = new ConcurrentDictionary<ulong, IContact>();
        private readonly ConcurrentDictionary</*guid*/ ulong, HashSet<ulong>> contactSubscriptions = new ConcurrentDictionary<ulong, HashSet<ulong>>();

        private readonly UpdateTimer saveTimer = new UpdateTimer(60d, true);

        /// <summary>
        /// Initialise the <see cref="IGlobalContactManager"/>.
        /// </summary>
        public void Initialise()
        {
            // Note: This makes the first ID equal temporaryMod + 1.This is because the client needs a value with a minimum of 281474976710656 for the Contact ID otherwise it is flagged
            // as a temporary contact.
            // TODO: Fix this to also include temporary contacts?
            ulong maxDbId = DatabaseManager.Instance.GetDatabase<CharacterDatabase>().GetNextContactId();
            nextContactId = maxDbId > temporaryMod ? maxDbId + 1ul : maxDbId + temporaryMod + 1ul;
        }

        /// <summary>
        /// Shutdown <see cref="IGlobalContactManager"/> and any related resources.
        /// </summary>
        /// <remarks>
        /// This will force save all pending contacts.
        /// </remarks>
        public void Shutdown()
        {
            log.Info("Shutting down contacts manager...");

            SaveContacts();
        }

        /// <summary>
        /// Called in the main update method. Used to run tasks to sync <see cref="Contact"/>to database.
        /// </summary>
        /// <param name="lastTick"></param>
        public void Update(double lastTick)
        {
            saveTimer.Update(lastTick);

            if (saveTimer.HasElapsed)
            {
                SaveContacts();
                saveTimer.Reset();
            }
        }

        private void SaveContacts()
        {
            var tasks = new List<Task>();

            foreach (Contact contact in contactsToSave.Values.ToList())
                tasks.Add(DatabaseManager.Instance.GetDatabase<CharacterDatabase>().Save(contact.Save));

            Task.WaitAll(tasks.ToArray());

            contactsToSave.Clear();
        }

        /// <summary>
        /// Get all <see cref="IContact"/> request responses which have been queued for save
        /// </summary>
        public IEnumerable<IContact> GetQueuedRequests(ulong ownerId)
        {
            foreach (IContact contact in contactsToSave.Values)
            {
                if (contact.OwnerId == ownerId)
                {
                    contactsToSave.TryRemove(contact.Id, out IContact contactRequest);
                    yield return contactRequest;
                }
            }
        }

        /// <summary>
        /// Declines the pending <see cref="IContact"/> request.
        /// </summary>
        public void DeclineRequest(IContact contact)
        {
            IPlayer playerContact = PlayerManager.Instance.GetPlayer(contact.OwnerId);
            if (playerContact != null)
            {
                // Alert the User and have them update their own Contact
                playerContact.ContactManager.ContactRequestDeclined(contact.Id);
            }
            else
            {
                // Player is offline, so we're going to save from the Global Contact Manager instead.
                contact.DeclineRequest();
                contactsToSave.TryAdd(contact.Id, contact);
            }
        }

        public void AcceptRequest(IContact contact)
        {
            // Process Contact Request if user is online
            IPlayer playerContact = PlayerManager.Instance.GetPlayer(contact.OwnerId);
            if (playerContact != null)
            {
                playerContact.ContactManager.ContactRequestAccepted(contact.Id);
                NotifySubscriber(contact.OwnerId, contact.ContactId);
            }
            else
            {
                // Player is offline, so we're going to save from the Global Contact Manager instead.
                contact.AcceptRequest();
                contactsToSave.TryAdd(contact.Id, contact);
            }
        }

        /// <summary>
        /// Subscribe player with the provided Character ID to notifications if & when users in the Character ID List come online
        /// </summary>
        public void SubscribeTo(ulong characterId, IEnumerable<ulong> characterIdList)
        {
            if (contactSubscriptions.ContainsKey(characterId))
                contactSubscriptions[characterId].UnionWith(characterIdList);
            else
                contactSubscriptions.TryAdd(characterId, characterIdList.ToHashSet());
        }

        /// <summary>
        /// Unsubscribe player with the provided Character ID to notifications if & when users in the Character ID List come online
        /// </summary>
        public void UnsubscribeFrom(ulong characterId, List<ulong> characterIdList)
        {
            if (!contactSubscriptions.ContainsKey(characterId))
                throw new ArgumentOutOfRangeException($"Cannot unsubscribe from characters when the subscriber doesn't exist.");

            contactSubscriptions[characterId].RemoveWhere(i => characterIdList.Contains(i));
        }

        /// <summary>
        /// Remove subscriber with the provided Character ID from all previous subscriptions
        /// </summary>
        /// <param name="characterId"></param>
        public void RemoveSubscriber(ulong characterId)
        {
            if (!contactSubscriptions.ContainsKey(characterId))
                throw new ArgumentOutOfRangeException($"Cannot unsubscribe from characters when the subscriber doesn't exist.");

            contactSubscriptions.Remove(characterId, out _);
        }

        /// <summary>
        /// Notifies online <see cref="IContact"/> owners, that the player has logged in or out.
        /// </summary>
        public void NotifySubscribers(ulong characterId, bool loggingOut = false)
        {
            foreach ((ulong subscriberId, HashSet<ulong> subscriptions) in contactSubscriptions.Where(i => i.Value.Contains(characterId)))
                NotifySubscriber(subscriberId, characterId, loggingOut);
        }

        /// <summary>
        /// Notifies online <see cref="IContact"/> owners, that the player has logged in or out.
        /// </summary>
        public void NotifySubscriber(ulong subscriberId, ulong contactCharacterId, bool loggingOut = false)
        {
            IPlayer playerContact = PlayerManager.Instance.GetPlayer(subscriberId);
            if (playerContact != null)
                playerContact.Session.EnqueueMessageEncrypted(new ServerContactsUpdateStatus
                {
                    PlayerIdentity = new TargetPlayerIdentity
                    {
                        RealmId = RealmContext.Instance.RealmId,
                        CharacterId = contactCharacterId
                    },
                    LastOnlineInDays = loggingOut ? 0.00069f : 0
                });
        }

        /// <summary>
        /// Attempt to send a <see cref="IContact"/> request to it's target Player
        /// </summary>
        /// <param name="contactRequest">The Contact request you wish to send</param>
        public void SendRequestToPlayer(IContact contactRequest)
        {
            // Process Pending Request if user is online
            IPlayer playerContact = PlayerManager.Instance.GetPlayer(contactRequest.ContactId);
            if (playerContact != null)
                playerContact.ContactManager.ContactRequestCreate(contactRequest);
        }

        /// <summary>
        /// Remove a pending <see cref="IContact"/> request from an online player
        /// </summary>
        /// <param name="contactRequest">Contact request to remove</param>
        public void TryRemoveRequestFromOnlineUser(IContact contactRequest)
        {
            IPlayer playerContact = PlayerManager.Instance.GetPlayer(contactRequest.ContactId);
            if (playerContact != null)
                SendContactRequestRemove(playerContact, contactRequest.Id);
        }

        /// <summary>
        /// Send a contact request removal packet to a player
        /// </summary>
        /// <param name="player">IPlayer to send the contact request removal packet to</param>
        /// <param name="contactId">Contact ID of the request to be removed</param>
        private static void SendContactRequestRemove(IPlayer player, ulong contactId)
        {
            player.Session.EnqueueMessageEncrypted(new ServerContactsRequestRemove
            {
                ContactId = contactId
            });
        }
    }
}
