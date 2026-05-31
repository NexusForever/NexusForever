using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Friendship.Model;
using NexusForever.Database.Friendship.Repository;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public class FriendAccountManager
    {
        #region Dependency Injection

        private readonly IServiceProvider _serviceProvider;
        private readonly AccountFriendRepository _repository;

        public FriendAccountManager(
            IServiceProvider serviceProvider,
            AccountFriendRepository repository)
        {
            _serviceProvider = serviceProvider;
            _repository = repository;
        }

        #endregion

        /// <summary>
        /// Get a <see cref="FriendAccountInvite"/> with the specified id.
        /// </summary>
        /// <param name="id">Id of the <see cref="FriendAccountInvite"/> to return.</param>
        public async Task<FriendAccountInvite> GetFriendInviteAsync(ulong id)
        {
            FriendAccountInviteModel model = await _repository.GetFriendInviteAsync(id);
            if (model == null)
                return null;

            FriendAccountInvite invite = _serviceProvider.GetRequiredService<FriendAccountInvite>();
            invite.Initialise(model);
            return invite;
        }

        /// <summary>
        /// Create a new <see cref="FriendAccountInvite"/> and add it to the repository.
        /// </summary>
        /// <param name="inviter">Id of the inviter initiating the friend invite.</param>
        /// <param name="invitee">Id of the invitee receiving the friend invite.</param>
        /// <param name="note">Optional note for the friend invite.</param>
        /// <returns>The created <see cref="FriendAccountInvite"/>.</returns>
        public FriendAccountInvite CreateFriendInvite(uint inviter, uint invitee, string note)
        {
            var invite = _serviceProvider.GetRequiredService<FriendAccountInvite>();
            invite.Initialise(inviter, invitee, note);
            _repository.AddFriendInvite(invite.Model);
            return invite;
        }

        /// <summary>
        /// Remove a <see cref="FriendAccountInvite"/> from the repository.
        /// </summary>
        /// <param name="invite">Invite to remove.</param>
        public void RemoveFriendInvite(FriendAccountInvite invite)
        {
            _repository.RemoveFriendInvite(invite.Model);
        }

        /// <summary>
        /// Get a <see cref="FriendAccount"/> with the specified id.
        /// </summary>
        /// <param name="id">Id of the <see cref="FriendAccount"/> to return.</param>
        /// <returns></returns>
        public async Task<FriendAccount> GetFriendAsync(ulong id)
        {
            FriendAccountModel model = await _repository.GetFriendAsync(id);
            if (model == null)
                return null;

            FriendAccount friend = _serviceProvider.GetRequiredService<FriendAccount>();
            friend.Initialise(model);
            return friend;
        }

        /// <summary>
        /// Create a new <see cref="FriendAccount"/> and add it to the repository.
        /// </summary>
        /// <param name="inviter">Id of the inviter initiating the friendship.</param>
        /// <param name="invitee">Id of the invitee receiving the friendship.</param>
        /// <returns>The created <see cref="FriendAccount"/>.</returns>
        public FriendAccount CreateFriend(uint inviter, uint invitee)
        {
            var friend = _serviceProvider.GetRequiredService<FriendAccount>();
            friend.Initialise(inviter, invitee);
            _repository.AddFriend(friend.Model);
            return friend;
        }

        /// <summary>
        /// Remove a <see cref="FriendAccount"/> from the repository.
        /// </summary>
        /// <param name="friend">Friend to remove.</param>
        public void RemoveFriend(FriendAccount friend)
        {
            _repository.RemoveFriend(friend.Model);
        }
    }
}
