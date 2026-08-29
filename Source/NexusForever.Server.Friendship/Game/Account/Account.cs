using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using NexusForever.Database;
using NexusForever.Database.Friendship.Model;
using NexusForever.Game.Static.Chat;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Server.Friendship.Game.Character;
using NexusForever.Server.Friendship.Game.Friend;
using NexusForever.Server.Friendship.Network.Internal;
using InternalFriend = NexusForever.Network.Internal.Message.Friendship.Shared.FriendAccount;

namespace NexusForever.Server.Friendship.Game.Account
{
    public class Account : IWrappedModel<AccountModel>
    {
        public AccountModel Model { get; private set; }

        public uint Id => Model.AccountId;

        public string Email => Model.Email;

        public string Nickname
        {
            get => Model.Nickname;
            private set => Model.Nickname = value;
        }

        public string Status
        {
            get => Model.Status;
            private set => Model.Status = value;
        }

        public AccountPresenceState Presence
        {
            get => Model.Presence;
            private set => Model.Presence = value;
        }

        public bool BlockAccountFriendRequests
        {
            get => Model.BlockAccountFriendRequests;
            private set => Model.BlockAccountFriendRequests = value;
        }

        public bool InvitePrivilegesSuspended => Model.InvitePrivilegesSuspended;

        public Identity ActiveCharacterIdentity
        {
            get
            {
                if (Model.ActiveCharacterId == null || Model.ActiveRealmId == null)
                    return null;

                return new Identity
                {
                    Id      = Model.ActiveCharacterId.Value,
                    RealmId = Model.ActiveRealmId.Value
                };
            }
            private set
            {
                Model.ActiveCharacterId = value?.Id;
                Model.ActiveRealmId = value?.RealmId;
            }
        }

        public DateTime? LastOnline
        {
            get => Model.LastOnline;
            private set => Model.LastOnline = value;
        }

        private readonly Dictionary<ulong, AccountFriendInvite> _invites = [];
        private readonly Dictionary<ulong, AccountFriendInvitePending> _invitesPending = [];

        private readonly Dictionary<ulong, AccountFriend> _friends = [];
        private readonly Dictionary<ulong, AccountFriendInverse> _friendsInverse = [];

        #region Dependency Injection

        private readonly ILogger<Account> _log;
        private readonly IServiceProvider _serviceProvider;
        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly CharacterManager _characterManager;

        public Account(
            ILogger<Account> log,
            IServiceProvider serviceProvider,
            OutboxMessagePublisher messagePublisher,
            CharacterManager characterManager)
        {
            _log              = log;
            _serviceProvider  = serviceProvider;
            _messagePublisher = messagePublisher;
            _characterManager = characterManager;
        }

        #endregion

        /// <summary>
        /// Initialise the <see cref="Account"/> with the supplied model.
        /// </summary>
        /// <param name="model">Model to initialise account.</param>
        public void Initialise(AccountModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("Account is already initialised.");

            Model = model;

            // initialise invites the account has yet to accept from other accounts
            foreach (AccountFriendInviteModel accountFriendInviteModel in model.FriendInvites)
            {
                var accountFriendInvite = _serviceProvider.GetRequiredService<AccountFriendInvite>();
                accountFriendInvite.Initialise(accountFriendInviteModel);
                _invites.Add(accountFriendInvite.FriendAccountInviteId, accountFriendInvite);
            }

            // initialise invites the account has sent to other accounts that are pending acceptance
            foreach (AccountFriendInvitePendingModel accountFriendInvitePendingModel in model.FriendInvitesPending)
            {
                var accountFriendInvitePending = _serviceProvider.GetRequiredService<AccountFriendInvitePending>();
                accountFriendInvitePending.Initialise(accountFriendInvitePendingModel);
                _invitesPending.Add(accountFriendInvitePending.FriendAccountInviteId, accountFriendInvitePending);
            }

            // initialise friends the account has accepted
            foreach (AccountFriendModel accountFriendModel in model.Friends)
            {
                var accountFriend = _serviceProvider.GetRequiredService<AccountFriend>();
                accountFriend.Initialise(accountFriendModel);
                _friends.Add(accountFriend.FriendAccountId, accountFriend);
            }

            // initialise inverse friends, these are other accounts that have accepted the account as a friend
            foreach (AccountFriendInverseModel accountFriendInverseModel in model.FriendsInverse)
            {
                var accountFriendInverse = _serviceProvider.GetRequiredService<AccountFriendInverse>();
                accountFriendInverse.Initialise(accountFriendInverseModel);
                _friendsInverse.Add(accountFriendInverse.FriendAccountId, accountFriendInverse);
            }

            _log.LogTrace("Initialised account {AccountId}.", Id);
        }

        /// <summary>
        /// Get the active character for the <see cref="Account"/>.
        /// </summary>
        /// <returns>The active <see cref="Character.Character"/> for the <see cref="Account"/>, or null if no character is active.</returns>
        public async Task<Character.Character> GetActiveCharacterAsync()
        {
            if (ActiveCharacterIdentity == null)
                return null;

            return await _characterManager.GetCharacterAsync(ActiveCharacterIdentity);
        }

        /// <summary>
        /// Set the nickname for the <see cref="Account"/>.
        /// </summary>
        /// <remarks>
        /// The nickname is the public name shown to all friends of the account instead of the account email.
        /// </remarks>
        /// <param name="nickname">The new nickname to set for the account.</param>
        /// <returns></returns>
        public async Task SetNicknameAsync(string nickname)
        {
            Nickname = nickname;

            _log.LogTrace("Set nickname for account {AccountId} to {Nickname}.", Id, nickname);

            await _messagePublisher.PublishAsync(new FriendshipAccountNicknameUpdatedMessage
            {
                Account        = await this.ToInternalAccountAsync(),
                FriendsInverse = await GetInternalFriendsInverseAsync()
            });
        }

        /// <summary>
        /// Set the status for the <see cref="Account"/>.
        /// </summary>
        /// <remarks>
        /// The status is the public note shown to all friends of the account.
        /// </remarks>
        /// <param name="status">The new status to set for the account, or null to remove the status.</param>
        public async Task SetStatusAsync(string status)
        {
            Status = status;

            if (status == null)
                _log.LogTrace("Removed status for account {AccountId}.", Id);
            else
                _log.LogTrace("Set status for account {AccountId} to {Status}.", Id, status);

            await _messagePublisher.PublishAsync(new FriendshipAccountStatusUpdatedMessage
            {
                Account        = await this.ToInternalAccountAsync(),
                FriendsInverse = await GetInternalFriendsInverseAsync()
            });
        }

        /// <summary>
        /// Set the presence for the <see cref="Account"/>.
        /// </summary>
        /// <param name="presence">The <see cref="AccountPresenceState"/> state to set for the account.</param>
        /// <returns></returns>
        public async Task SetPresenceAsync(AccountPresenceState presence)
        {
            Presence = presence;

            _log.LogTrace("Set presence for account {AccountId} to {Presence}.", Id, presence);

            await _messagePublisher.PublishAsync(new FriendshipAccountPresenceUpdatedMessage
            {
                Account        = await this.ToInternalAccountAsync(),
                FriendsInverse = await GetInternalFriendsInverseAsync()
            });
        }

        /// <summary>
        /// Set the active character for the <see cref="Account"/>.
        /// </summary>
        /// <param name="identity">The character to set as the active character on the account, or null to remove the active character.</param>
        /// <returns></returns>
        public async Task SetActiveCharacterAsync(Identity identity)
        {
            ActiveCharacterIdentity = identity;
            Presence                = ActiveCharacterIdentity != null ? AccountPresenceState.Available : AccountPresenceState.Invisible;

            if (identity == null)
                _log.LogTrace("Removed active character for account {AccountId}.", Id);
            else
                _log.LogTrace("Set active character for account {AccountId} to {CharacterIdentity}.", Id, identity);

            await _messagePublisher.PublishAsync(new FriendshipAccountUpdatedMessage
            {
                Account        = await this.ToInternalAccountAsync(),
                FriendsInverse = await GetInternalFriendsInverseAsync()
            });
        }

        /// <summary>
        /// Set the last online for the <see cref="Account"/>.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> to set as the last online value, or null to remove the last online datetime.</param>
        public async Task SetLastOnline(DateTime? dateTime)
        {
            LastOnline = dateTime;

            if (dateTime == null)
                _log.LogTrace("Removed last online for account {AccountId}.", Id);
            else
                _log.LogTrace("Set last online for account {AccountId} to {LastOnline}.", Id, dateTime);

            await _messagePublisher.PublishAsync(new FriendshipAccountLastOnlineUpdatedMessage
            {
                Account        = await this.ToInternalAccountAsync(),
                FriendsInverse = await GetInternalFriendsInverseAsync()
            });
        }

        /// <summary>
        /// Send the list of pending friend invites for the <see cref="Account"/>.
        /// </summary>
        public async Task SendFriendInvitesAsync()
        {
            var friendshipAccountInviteList = new FriendshipAccountInviteListMessage
            {
                Account = await this.ToInternalAccountAsync()
            };

            foreach (AccountFriendInvite accountInvite in _invites.Values)
            {
                FriendAccountInvite invite = await accountInvite.GetFriendInviteAsync();
                if (invite != null)
                    friendshipAccountInviteList.Invites.Add(await invite.ToInternalFriendInvite());
            }

            await _messagePublisher.PublishAsync(friendshipAccountInviteList);
        }

        /// <summary>
        /// Gets the number of pending friend invites for the <see cref="Account"/>.
        /// </summary>
        public uint GetFriendInviteCount()
        {
            return (uint)_invites.Count;
        }

        /// <summary>
        /// Return a pending <see cref="FriendAccountInvite"/> that belongs to the <see cref="Account"/> with the specified invite id.
        /// </summary>
        /// <param name="inviteId">Id of the invite to retrieve.</param>
        /// <returns></returns>
        public async Task<FriendAccountInvite> GetFriendInviteAsync(ulong inviteId)
        {
            if (!_invites.TryGetValue(inviteId, out AccountFriendInvite accountInvite))
                return null;

            return await accountInvite.GetFriendInviteAsync();
        }

        /// <summary>
        /// Add a new <see cref="FriendAccountInvite"/> to the <see cref="Account"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="FriendAccountInvite"/> should have a valid id, this should come from the database.
        /// The use of a transaction should be used to ensure the invite is created and added to the account atomically.
        /// </remarks>
        /// <param name="invite">Invite to add to the <see cref="Account"/>.</param>
        /// <exception cref="ArgumentException">Thrown if the invite does not have a valid identifier.</exception>
        public async Task AddFriendInviteAsync(FriendAccountInvite invite)
        {
            if (invite.Id == 0)
                throw new ArgumentException("Invite must have a valid id.", nameof(invite));

            var accountFriendInvite = _serviceProvider.GetRequiredService<AccountFriendInvite>();
            accountFriendInvite.Initialise(invite.Id);

            Model.FriendInvites.Add(accountFriendInvite.Model);
            _invites.Add(accountFriendInvite.FriendAccountInviteId, accountFriendInvite);

            _log.LogTrace("Added friend invite {InviteId} to account {AccountId}.", invite.Id, Id);

            await SendFriendInvitesAsync();
        }

        /// <summary>
        /// Remove a pending <see cref="FriendAccountInvite"/> from the <see cref="Account"/>.
        /// </summary>
        /// <param name="invite">Invite to remove from the <see cref="Account"/>.</param>
        public async Task RemoveFriendInviteAsync(FriendAccountInvite invite)
        {
            if (!_invites.Remove(invite.Id, out AccountFriendInvite accountInvite))
                return;

            Model.FriendInvites.Remove(accountInvite.Model);

            _log.LogTrace("Removed friend invite {InviteId} from account {AccountId}.", invite.Id, Id);

            await _messagePublisher.PublishAsync(new FriendshipAccountInviteRemovedMessage
            {
                FriendInvite = await invite.ToInternalFriendInvite()
            });
        }

        /// <summary>
        /// Return the pending <see cref="FriendAccountInvite"/> for the specified account id.
        /// </summary>
        /// <param name="accountId">The account id for which to retrieve the pending friend invite.</param>
        public async Task<FriendAccountInvite> GetFriendInvitePendingAsync(uint accountId)
        {
            foreach (AccountFriendInvitePending pendingInvite in _invitesPending.Values)
            {
                FriendAccountInvite invite = await pendingInvite.GetFriendInviteAsync();
                if (invite.InviteeAccountId == accountId)
                    return invite;
            }

            return null;
        }

        /// <summary>
        /// Add a new pending <see cref="FriendAccountInvite"/> to the <see cref="Account"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="FriendAccountInvite"/> should have a valid id, this should come from the database.
        /// The use of a transaction should be used to ensure the invite is created and added to the account atomically.
        /// </remarks>
        /// <param name="invite">Invite to add to the <see cref="Account"/>.</param>
        /// <exception cref="ArgumentException">Thrown if the invite does not have a valid identifier.</exception>
        public void AddFriendPendingInvite(FriendAccountInvite invite)
        {
            if (invite.Id == 0)
                throw new ArgumentException("Invite must have a valid id.", nameof(invite));

            var accountFriendInvitePending = _serviceProvider.GetRequiredService<AccountFriendInvitePending>();
            accountFriendInvitePending.Initialise(invite.Id);

            Model.FriendInvitesPending.Add(accountFriendInvitePending.Model);
            _invitesPending.Add(accountFriendInvitePending.FriendAccountInviteId, accountFriendInvitePending);
            
            _log.LogTrace("Added pending friend invite {InviteId} to account {AccountId}.", invite.Id, Id);
        }

        /// <summary>
        /// Remove a pending <see cref="FriendAccountInvite"/> from the <see cref="Account"/>.
        /// </summary>
        /// <param name="invite">Invite pending to remove from the <see cref="Account"/>.</param>
        public void RemoveFriendInvitePending(FriendAccountInvite invite)
        {
            if (!_invitesPending.TryGetValue(invite.Id, out AccountFriendInvitePending accountInvitePending))
                return;

            Model.FriendInvitesPending.Remove(accountInvitePending.Model);
            _invitesPending.Remove(accountInvitePending.FriendAccountInviteId);

            _log.LogTrace("Removed pending friend invite {InviteId} from account {AccountId}.", invite.Id, Id);
        }

        /// <summary>
        /// Send the list of friends for the <see cref="Account"/>.
        /// </summary>
        public async Task SendFriendsAsync()
        {
            var friendshipAccountList = new FriendshipAccountListMessage
            {
                Account = await this.ToInternalAccountAsync()
            };

            foreach (AccountFriend accountFriend in _friends.Values)
            {
                FriendAccount friend = await accountFriend.GetFriendAsync();
                if (friend != null)
                    friendshipAccountList.Friends.Add(await friend.ToInternalFriendAsync());
            }

            await _messagePublisher.PublishAsync(friendshipAccountList);
        }

        /// <summary>
        /// Get the number of friends for the <see cref="Account"/>.
        /// </summary>
        public uint GetFriendCount()
        {
            return (uint)_friends.Count;
        }

        /// <summary>
        /// Return a <see cref="FriendAccount"/> that belongs to the <see cref="Account"/> with the specified friend id.
        /// </summary>
        /// <param name="friendId">Id of the friend to retrieve.</param>
        public async Task<FriendAccount> GetFriendAsync(ulong friendId)
        {
            if (!_friends.TryGetValue(friendId, out AccountFriend accountFriend))
                return null;

            return await accountFriend.GetFriendAsync();
        }

        /// <summary>
        /// Return a <see cref="FriendAccount"/> that belongs to the <see cref="Account"/> with the specified account id.
        /// </summary>
        /// <param name="accountId">Id of the account to retrieve the friend for.</param>
        public async Task<FriendAccount> GetFriendByAccountId(uint accountId)
        {
            foreach (AccountFriend accountFriend in _friends.Values)
            {
                FriendAccount friend = await accountFriend.GetFriendAsync();
                if (friend.InviteeAccountId == accountId)
                    return friend;
            }

            return null;
        }

        /// <summary>
        /// Return a collection of other accounts that have added the <see cref="Account"/> as a friend.
        /// </summary>
        public async Task<IEnumerable<FriendAccount>> GetFriendsInverseAsync()
        {
            var list = new List<FriendAccount>();
            foreach (AccountFriendInverse accountFriendInverse in _friendsInverse.Values)
                list.Add(await accountFriendInverse.GetFriendAsync());

            return list;
        }

        /// <summary>
        /// Return a collection of other accounts that have added the <see cref="Account"/> as a friend.
        /// </summary>
        /// <remarks>
        /// This is a helper method to return inverse friends in a format suitable for sending in internal messages.
        /// </remarks>
        public async Task<List<InternalFriend>> GetInternalFriendsInverseAsync()
        {
            var friends = new List<InternalFriend>();
            foreach (FriendAccount friend in await GetFriendsInverseAsync())
                friends.Add(await friend.ToInternalFriendAsync());

            return friends;
        }

        /// <summary>
        /// Add a new <see cref="FriendAccount"/> to the <see cref="Account"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="FriendAccount"/> should have a valid id, this should come from the database.
        /// The use of a transaction should be used to ensure the friend is created and added to the account atomically.
        /// </remarks>
        /// <param name="friend">Friend to add to the <see cref="Account"/>.</param>
        /// <exception cref="ArgumentException">Thrown if the friend does not have a valid identifier.</exception>
        public async Task AddFriendAsync(FriendAccount friend)
        {
            if (friend.Id == 0)
                throw new ArgumentException("Friend must have a valid id.", nameof(friend));

            var accountFriend = _serviceProvider.GetRequiredService<AccountFriend>();
            accountFriend.Initialise(friend.Id);

            if (!_friends.TryAdd(accountFriend.FriendAccountId, accountFriend))
                return;

            Model.Friends.Add(accountFriend.Model);

            _log.LogTrace("Added friend {FriendAccountId} to account {AccountId}.", friend.InviteeAccountId, Id);

            await SendFriendsAsync();
        }

        /// <summary>
        /// Remove an existing <see cref="FriendAccount"/> from the <see cref="Account"/>.
        /// </summary>
        /// <param name="friend">Friend to remove from the <see cref="Account"/>.</param>
        public async Task RemoveFriendAsync(FriendAccount friend)
        {
            if (!_friends.Remove(friend.Id, out AccountFriend accountFriend))
                return;

            Model.Friends.Remove(accountFriend.Model);

            _log.LogTrace("Removed friend {FriendAccountId} from account {AccountId}.", friend.Id, Id);

            await _messagePublisher.PublishAsync(new FriendshipAccountRemovedMessage
            {
                FriendAccount = await friend.ToInternalFriendAsync()
            });
        }

        /// <summary>
        /// Add a new <see cref="FriendAccount"/> to the <see cref="Account"/> as an inverse friend.
        /// </summary>
        /// <remarks>
        /// An inverse friend represents another account that has added this account as a friend.
        /// </remarks>
        /// <param name="friend">Friend to add as an inverse friend.</param>
        /// <exception cref="ArgumentException">Thrown if the friend does not have a valid identifier.</exception>
        public void AddFriendInverse(FriendAccount friend)
        {
            if (friend.Id == 0)
                throw new ArgumentException("Friend must have a valid id.", nameof(friend));

            var accountFriendInverse = _serviceProvider.GetRequiredService<AccountFriendInverse>();
            accountFriendInverse.Initialise(friend.Id);

            Model.FriendsInverse.Add(accountFriendInverse.Model);
            _friendsInverse.Add(friend.Id, accountFriendInverse);

            _log.LogTrace("Added inverse friend {FriendAccountId} to account {AccountId}.", friend.Id, Id);
        }

        /// <summary>
        /// Remove an existing inverse <see cref="FriendAccount"/> from the <see cref="Account"/>.
        /// </summary>
        /// <remarks>
        /// An inverse friend represents another account that has added this account as a friend.
        /// </remarks>
        /// <param name="friend">Friend to remove as an inverse friend.</param>
        public void RemoveFriendInverse(FriendAccount friend)
        {
            if (!_friendsInverse.Remove(friend.Id, out AccountFriendInverse accountFriendInverse))
                return;

            Model.FriendsInverse.Remove(accountFriendInverse.Model);

            _log.LogTrace("Removed inverse friend {FriendAccountId} from account {AccountId}.", friend.Id, Id);
        }
    }
}
