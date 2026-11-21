using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Friendship;
using NexusForever.Game.Static.Friendship;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public class FriendFactory
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly FriendManager _friendManager;

        public FriendFactory(
            FriendshipContext context,
            FriendManager friendManager)
        {
            _context       = context;
            _friendManager = friendManager;
        }

        #endregion

        /// <summary>
        /// Create a new <see cref="Friend"/>, add it to the repository and add friendship to both the inviter and invitee.
        /// </summary>
        /// <remarks>
        /// This will call <see cref="DbContext.SaveChangesAsync"/> on the <see cref="FriendshipContext"/> to persist the new friend to the database.
        /// This is required to generate the friend id, which is needed to add it to the inviter and invitee.
        /// It is recommended you use a database transaction to keep the creation of the friend and the addition to the inviter and invitee atomic.
        /// </remarks>
        /// <param name="inviter">Character initiating the friendship.</param>
        /// <param name="invitee">Character receiving the friendship.</param>
        /// <param name="type">The type of friendship.</param>
        /// <returns>The created <see cref="Friend"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <see cref="FriendshipType"/> is <see cref="FriendshipType.Account"/>. Use <see cref="FriendAccountFactory.CreateFriendAsync(Account.Account, Account.Account)"/> instead.</exception>
        public async Task<Friend> CreateFriendAsync(Character.Character inviter, Character.Character invitee, FriendshipType type)
        {
            // use FriendAccountFactory
            if (type == FriendshipType.Account)
                throw new ArgumentOutOfRangeException();

            Friend friend = _friendManager.CreateFriend(inviter.Identity, invitee.Identity, type);
            await _context.SaveChangesAsync();

            await inviter.AddFriendAsync(friend);
            invitee.AddFriendInverse(friend);

            return friend;
        }
    }
}
