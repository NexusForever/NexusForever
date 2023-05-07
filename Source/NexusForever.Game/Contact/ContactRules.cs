using NexusForever.Game.Static.Contact;
using NexusForever.Game.Abstract.Contact;
using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Contact
{
    public partial class GlobalContactManager
    {
        /// <summary>
        /// Checks to see if the target character can become a contact
        /// </summary>
        public bool CanBeContact(IPlayer player, ulong recipientId, ContactType type, Dictionary</*guid*/ ulong, IContact> playerContacts)
        {
            return CanBeContactResult(player, recipientId, type, playerContacts) == ContactResult.Ok;
        }

        /// <summary>
        /// Checks to see if the target character can become a contact, and returns a <see cref="ContactResult"/> appropriately
        /// </summary>
        public ContactResult CanBeContactResult(IPlayer player, ulong receipientId, ContactType type, Dictionary</*guid*/ ulong, IContact> playerContacts)
        {
            Dictionary<ContactType, uint> maxTypeMap = new Dictionary<ContactType, uint>
            {
                { ContactType.Friend, GetMaxFriends() },
                { ContactType.Account, GetMaxFriends() },
                { ContactType.Ignore, GetMaxIgnored() },
                { ContactType.Rival, GetMaxRivals() }
            };
            Dictionary<ContactType, ContactResult> maxTypeResponseMap = new Dictionary<ContactType, ContactResult>
            {
                { ContactType.Friend, ContactResult.MaxFriends },
                { ContactType.Account, ContactResult.MaxFriends },
                { ContactType.Ignore, ContactResult.MaxIgnored },
                { ContactType.Rival, ContactResult.MaxRivals }
            };
            // Check player isn't capped for this Contact Type
            if (type != ContactType.FriendAndRival && playerContacts.Values.Where(c => c.Id == player.CharacterId && c.Type == type && !c.IsPendingDelete).ToList().Count > maxTypeMap[type])
            {
                return maxTypeResponseMap[type];
            }
            else if (type == ContactType.FriendAndRival)
            {
                // Check both maximum counts are checked
                if (playerContacts.Values.Where(c => c.Type == ContactType.Friend && !c.IsPendingDelete).ToList().Count > maxTypeMap[ContactType.Friend])
                {
                    return maxTypeResponseMap[ContactType.Friend];
                }
                else if (playerContacts.Values.Where(c => c.Type == ContactType.Rival && !c.IsPendingDelete).ToList().Count > maxTypeMap[ContactType.Rival])
                {
                    return maxTypeResponseMap[ContactType.Rival];
                }
            }

            // Check recipient isn't already contact of requested type.
            if (playerContacts.Values.FirstOrDefault(c => c.ContactId == receipientId && c.Type == type && !c.IsPendingAcceptance && !c.IsPendingDelete) != null)
            {
                Dictionary<ContactType, ContactResult> alreadyContactResponseMap = new Dictionary<ContactType, ContactResult>
                {
                    { ContactType.Friend, ContactResult.PlayerAlreadyFriend },
                    { ContactType.Account, ContactResult.PlayerAlreadyFriend },
                    { ContactType.Ignore, ContactResult.PlayerAlreadyIgnored },
                    { ContactType.Rival, ContactResult.PlayerAlreadyRival }
                };

                return alreadyContactResponseMap[type];
            }

            // CHeck and make sure recipient doesn't have existing request
            if (type == ContactType.Friend || type == ContactType.FriendAndRival)
                if (playerContacts.Values.FirstOrDefault(c => c.ContactId == receipientId && c.Type == type && c.IsPendingAcceptance && !c.IsPendingDelete) != null)
                {
                    return ContactResult.PlayerQueuedRequests;
                }

            return ContactResult.Ok;
        }
    }
}
