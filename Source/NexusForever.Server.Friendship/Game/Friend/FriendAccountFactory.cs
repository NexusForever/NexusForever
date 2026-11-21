using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Friendship;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public class FriendAccountFactory
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly FriendAccountManager _friendManager;

        public FriendAccountFactory(FriendshipContext context,
            FriendAccountManager friendManager)
        {
            _context       = context;
            _friendManager = friendManager;
        }

        #endregion

        /// <summary>
        /// Create a new <see cref="FriendAccount"/>, add it to the repository and add friendship to both the inviter and invitee.
        /// </summary>
        /// <remarks>
        /// This will call <see cref="DbContext.SaveChangesAsync"/> on the <see cref="FriendshipContext"/> to persist the new friend to the database.
        /// This is required to generate the friend id, which is needed to add it to the inviter and invitee.
        /// It is recommended you use a database transaction to keep the creation of the friend and the addition to the inviter and invitee atomic.
        /// </remarks>
        /// <param name="inviter">Account initiating the friendship.</param>
        /// <param name="invitee">Account receiving the friendship.</param>
        /// <returns>The created <see cref="FriendAccount"/>.</returns>
        public async Task<FriendAccount> CreateFriendAsync(Account.Account inviter, Account.Account invitee)
        {
            FriendAccount friend = _friendManager.CreateFriend(inviter.Id, invitee.Id);
            await _context.SaveChangesAsync();

            await inviter.AddFriendAsync(friend);
            invitee.AddFriendInverse(friend);

            return friend;
        }
    }
}
