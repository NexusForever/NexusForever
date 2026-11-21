using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Friendship;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public class FriendInviteFactory
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly FriendManager _friendManager;

        public FriendInviteFactory(FriendshipContext context,
            FriendManager friendManager)
        {
            _context       = context;
            _friendManager = friendManager;
        }

        #endregion

        /// <summary>
        /// Create a new <see cref="FriendInvite"/>, add it to the repository and add invite to both the inviter and invitee.
        /// </summary>
        /// <remarks>
        /// This will call <see cref="DbContext.SaveChangesAsync"/> on the <see cref="FriendshipContext"/> to persist the new invite to the database.
        /// This is required to generate the invite id, which is needed to add it to the inviter and invitee.
        /// It is recommended you use a database transaction to keep the creation of the invite and the addition to the inviter and invitee atomic.
        /// </remarks>
        /// <param name="inviter">Character initiating the friend invite.</param>
        /// <param name="invitee">Character receiving the friend invite.</param>
        /// <param name="note">Optional note for the friend invite.</param>
        /// <returns>The created <see cref="FriendInvite"/>.</returns>
        public async Task<FriendInvite> CreateFriendInviteAsync(Character.Character inviter, Character.Character invitee, string note)
        {
            FriendInvite invite = _friendManager.CreateFriendInvite(inviter.Identity, invitee.Identity, note);
            await _context.SaveChangesAsync();

            inviter.AddFriendInvitePending(invite);
            await invitee.AddFriendInviteAsync(invite);

            return invite;
        }
    }
}
