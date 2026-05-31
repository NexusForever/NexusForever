using NexusForever.Database.Friendship;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public class FriendAccountInviteFactory
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly FriendAccountManager _friendManager;

        public FriendAccountInviteFactory(
            FriendshipContext context,
            FriendAccountManager friendManager)
        {
            _context       = context;
            _friendManager = friendManager;
        }

        #endregion

        /// <summary>
        /// Create a new <see cref="FriendAccountInvite"/>, add it to the repository and add invite to both the inviter and invitee.
        /// </summary>
        /// <remarks>
        /// This will call <see cref="DbContext.SaveChangesAsync"/> on the <see cref="FriendshipContext"/> to persist the new invite to the database.
        /// This is required to generate the invite id, which is needed to add it to the inviter and invitee.
        /// It is recommended you use a database transaction to keep the creation of the invite and the addition to the inviter and invitee atomic.
        /// </remarks>
        /// <param name="inviter">Account initiating the friend invite.</param>
        /// <param name="invitee">Account receiving the friend invite.</param>
        /// <param name="note">Optional note for the friend invite.</param>
        /// <returns>The created <see cref="FriendAccountInvite"/>.</returns>
        public async Task<FriendAccountInvite> CreateFriendInviteAsync(Account.Account inviter, Account.Account invitee, string note)
        {
            FriendAccountInvite invite = _friendManager.CreateFriendInvite(inviter.Id, invitee.Id, note);
            await _context.SaveChangesAsync();

            await invitee.AddFriendInviteAsync(invite);
            inviter.AddFriendPendingInvite(invite);

            return invite;
        }
    }
}
