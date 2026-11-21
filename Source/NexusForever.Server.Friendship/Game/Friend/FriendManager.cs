using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Friendship.Model;
using NexusForever.Database.Friendship.Repository;
using NexusForever.Game.Static.Friendship;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public class FriendManager
    {
        #region Dependency Injection

        private readonly IServiceProvider _serviceProvider;
        private readonly FriendRepository _repository;

        public FriendManager(
            IServiceProvider serviceProvider,
            FriendRepository repository)
        {
            _serviceProvider = serviceProvider;
            _repository      = repository;
        }

        #endregion

        /// <summary>
        /// Get a <see cref="FriendInvite"/> with the specified id.
        /// </summary>
        /// <param name="id">Id of the <see cref="FriendInvite"/> to return.</param>
        public async Task<FriendInvite> GetFriendInviteAsync(ulong id)
        {
            FriendInviteModel model = await _repository.GetFriendInviteAsync(id);
            if (model == null)
                return null;

            FriendInvite invite = _serviceProvider.GetRequiredService<FriendInvite>();
            invite.Initialise(model);
            return invite;
        }

        /// <summary>
        /// Create a new <see cref="FriendInvite"/> and add it to the repository.
        /// </summary>
        /// <param name="inviter">Identity of the inviter initiating the friend invite.</param>
        /// <param name="invitee">Identity of the invitee receiving the friend invite.</param>
        /// <param name="note">Optional note for the friend invite.</param>
        /// <returns>The created <see cref="FriendInvite"/>.</returns>
        public FriendInvite CreateFriendInvite(Identity inviter, Identity invitee, string note)
        {
            FriendInvite invite = _serviceProvider.GetRequiredService<FriendInvite>();
            invite.Initialise(inviter, invitee, note);
            _repository.AddFriendInvite(invite.Model);
            return invite;
        }

        /// <summary>
        /// Remove a <see cref="FriendInvite"/> from the repository.
        /// </summary>
        /// <param name="invite">Invite to remove.</param>
        public void RemoveFriendInvite(FriendInvite invite)
        {
            _repository.RemoveFriendInvite(invite.Model);
        }

        /// <summary>
        /// Get a <see cref="Friend"/> with the specified id.
        /// </summary>
        /// <param name="id">Id of the <see cref="Friend"/> to return.</param>
        /// <returns></returns>
        public async Task<Friend> GetFriendAsync(ulong id)
        {
            FriendModel model = await _repository.GetFriendAsync(id);
            if (model == null)
                return null;

            Friend friend = _serviceProvider.GetRequiredService<Friend>();
            friend.Initialise(model);
            return friend;
        }

        /// <summary>
        /// Create a new <see cref="Friend"/> and add it to the repository.
        /// </summary>
        /// <param name="inviter">Identity of the inviter initiating the friendship.</param>
        /// <param name="invitee">Identity of the invitee receiving the friendship.</param>
        /// <param name="type">Type of the friendship.</param>
        /// <returns>The created <see cref="Friend"/>.</returns>
        public Friend CreateFriend(Identity inviter, Identity invitee, FriendshipType type)
        {
            Friend friend = _serviceProvider.GetRequiredService<Friend>();
            friend.Initialise(inviter, invitee, type);
            _repository.AddFriend(friend.Model);

            return friend;
        }

        /// <summary>
        /// Remove a <see cref="Friend"/> from the repository.
        /// </summary>
        /// <param name="friend">Friend to remove.</param>
        public void RemoveFriend(Friend friend)
        {
            _repository.RemoveFriend(friend.Model);
        }
    }
}
