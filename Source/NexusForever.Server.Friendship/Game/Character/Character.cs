using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusForever.Database;
using NexusForever.Database.Friendship.Model;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Friendship;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Server.Friendship.Game.Account;
using NexusForever.Server.Friendship.Game.Friend;
using NexusForever.Server.Friendship.Network.Internal;

namespace NexusForever.Server.Friendship.Game.Character
{
    public class Character : IWrappedModel<CharacterModel>
    {
        public CharacterModel Model { get; private set; }

        public Identity Identity
        {
            get => new()
            {
                Id      = Model.CharacterId,
                RealmId = Model.RealmId
            };
        }

        public IdentityName IdentityName
        {
            get => new()
            {
                Name      = Model.Name,
                RealmName = Model.RealmName
            };
        }

        public uint AccountId => Model.AccountId;

        public Race Race => Model.Race;

        public Class Class => Model.Class;

        public NexusForever.Game.Static.Entity.Path Path => Model.Path;

        public Faction Faction => Model.Faction;

        public byte Level => (byte)(GetStat(Stat.Level) ?? 0u);

        public ushort WorldZoneId
        {
            get => Model.WorldZoneId;
            set => Model.WorldZoneId = value;
        }

        public uint WorldId
        {
            get => Model.WorldId;
            set => Model.WorldId = value;
        }

        public DateTime? LastOnline
        {
            get => Model.LastOnline;
            private set => Model.LastOnline = value;
        }


        private readonly Dictionary<ulong, CharacterFriendInvite> _invites = [];
        private readonly Dictionary<ulong, CharacterFriendInvitePending> _invitesPending = [];

        private readonly Dictionary<ulong, CharacterFriend> _friends = [];
        private readonly Dictionary<ulong, CharacterFriendInverse> _friendsInverse = [];

        private readonly Dictionary<Stat, CharacterStat> _stats = [];

        #region Dependency Injection

        private readonly ILogger<Character> _log;
        private readonly IServiceProvider _serviceProvider;
        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly AccountManager _accountManager;

        public Character(
            ILogger<Character> log,
            IServiceProvider serviceProvider,
            OutboxMessagePublisher messagePublisher,
            AccountManager accountManager)
        {
            _log              = log;
            _serviceProvider  = serviceProvider;
            _messagePublisher = messagePublisher;
            _accountManager   = accountManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="Character"/> with the suppled model.
        /// </summary>
        /// <param name="model">Model to initialise character.</param>
        public void Initialise(CharacterModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("Character is already initialised.");

            Model = model;

            // initialise invites the character has yet to accept from other characters
            foreach (CharacterFriendInviteModel inviteModel in model.FriendInvites)
            {
                CharacterFriendInvite invite = _serviceProvider.GetRequiredService<CharacterFriendInvite>();
                invite.Initialise(inviteModel);
                _invites.Add(invite.FriendInviteId, invite);
            }

            // initialise invites the character has sent to other characters that are pending acceptance
            foreach (CharacterFriendInvitePendingModel invitePendingModel in model.FriendInvitesPending)
            {
                CharacterFriendInvitePending invitePending = _serviceProvider.GetRequiredService<CharacterFriendInvitePending>();
                invitePending.Initialise(invitePendingModel);
                _invitesPending.Add(invitePendingModel.FriendInviteId, invitePending);
            }

            // initialise friends the character has accepted
            foreach (CharacterFriendModel friendModel in model.Friends)
            {
                CharacterFriend friend = _serviceProvider.GetRequiredService<CharacterFriend>();
                friend.Initialise(friendModel);
                _friends.Add(friend.FriendId, friend);
            }

            // initialise inverse friends, these are other characters that have accepted the character as a friend
            foreach (CharacterFriendInverseModel friendInverseModel in model.FriendsInverse)
            {
                CharacterFriendInverse friendInverse = _serviceProvider.GetRequiredService<CharacterFriendInverse>();
                friendInverse.Initialise(friendInverseModel);
                _friendsInverse.Add(friendInverse.FriendId, friendInverse);
            }

            foreach (CharacterStatModel statModel in model.Stats)
            {
                CharacterStat stat = _serviceProvider.GetRequiredService<CharacterStat>();
                stat.Initialise(statModel);
                _stats.Add(stat.Stat, stat);
            }

            _log.LogTrace("Initialised character {Identity}.", Identity);
        }

        /// <summary>
        /// Get the <see cref="Account.Account"/> that owns this character.
        /// </summary>
        /// <returns>The <see cref="Account.Account"/> that owns this character.</returns>
        public async Task<Account.Account> GetAccountAsync()
        {
            return await _accountManager.GetAccountAsync(AccountId);
        }

        /// <summary>
        /// Set the last online time for the <see cref="Character"/>.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> to set as the last online value, or null to remove the last online datetime.</param>
        public async Task SetLastOnline(DateTime? dateTime)
        {
            LastOnline = dateTime;

            if (dateTime == null)
                _log.LogTrace("Removed last online for character {Identity}.", Identity);
            else
                _log.LogTrace("Set last online for character {Identity} to {LastOnline}.", Identity, dateTime);

            var friendshipLastOnlineUpdated = new FriendshipLastOnlineUpdatedMessage
            {
                Character = this.ToInternalCharacter()
            };

            foreach (Friend.Friend friend in await GetFriendsInverseAsync())
                friendshipLastOnlineUpdated.FriendsInverse.Add(await friend.ToInternalFriendAsync());

            await _messagePublisher.PublishAsync(friendshipLastOnlineUpdated);
        }

        /// <summary>
        /// Set the level for the <see cref="Character"/>.
        /// </summary>
        /// <param name="level">The level to set for the <see cref="Character"/>.</param>
        public async Task SetLevelAsync(byte level)
        {
            SetStat(Stat.Level, level);

            var friendshipLevelUpdated = new FriendshipLevelUpdatedMessage()
            {
                Character = this.ToInternalCharacter()
            };

            foreach (Friend.Friend friend in await GetFriendsInverseAsync())
                friendshipLevelUpdated.FriendsInverse.Add(await friend.ToInternalFriendAsync());

            await _messagePublisher.PublishAsync(friendshipLevelUpdated);

            _log.LogTrace("Set level for character {Identity} to {Level}.", Identity, level);
        }

        public async Task SendFriendInvitesAsync()
        {
            var friendshipInviteList = new FriendshipInviteListMessage
            {
                Identity = Identity.ToInternalIdentity()
            };

            foreach (CharacterFriendInvite characterInvite in _invites.Values)
            {
                FriendInvite invite = await characterInvite.GetFriendInviteAsync();
                if (invite != null)
                    friendshipInviteList.Invites.Add(await invite.ToInternalFriendInvite());
            }

            await _messagePublisher.PublishAsync(friendshipInviteList);
        }

        /// <summary>
        /// Gets the number of pending friend invites for the <see cref="Character"/>.
        /// </summary>
        public uint GetFriendInviteCount()
        {
            return (uint)_invites.Count;
        }

        /// <summary>
        /// Send the list of pending friend invites for the <see cref="Character"/>.
        /// </summary>
        public async Task<FriendInvite> GetFriendInviteAsync(ulong id)
        {
            if (!_invites.TryGetValue(id, out CharacterFriendInvite characterFriendInvite))
                return null;

            return await characterFriendInvite.GetFriendInviteAsync();
        }

        /// <summary>
        /// Add a new <see cref="FriendInvite"/> to the <see cref="Character"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="FriendInvite"/> should have a valid id, this should come from the database.
        /// The use of a transaction should be used to ensure the invite is created and added to the character atomically.
        /// </remarks>
        /// <param name="invite">Invite to add to the <see cref="Character"/>.</param>
        /// <exception cref="ArgumentException">Thrown if the invite does not have a valid identifier.</exception>
        public async Task AddFriendInviteAsync(FriendInvite invite)
        {
            if (invite.Id == 0)
                throw new ArgumentException("Invite must have a valid id.", nameof(invite));

            var characterFriendInvite = _serviceProvider.GetRequiredService<CharacterFriendInvite>();
            characterFriendInvite.Initialise(invite.Id);

            Model.FriendInvites.Add(characterFriendInvite.Model);
            _invites.Add(invite.Id, characterFriendInvite);

            _log.LogTrace("Added friend invite {InviteId} to character {Identity}.", invite.Id, Identity);

            await SendFriendInvitesAsync();
        }

        /// <summary>
        /// Remove a pending <see cref="FriendInvite"/> from the <see cref="Character"/>.
        /// </summary>
        /// <param name="invite">Invite to remove from the <see cref="Character"/>.</param>
        public async Task RemoveFriendInviteAsync(FriendInvite invite)
        {
            if (!_invites.Remove(invite.Id, out CharacterFriendInvite characterFriendInvite))
                return;

            Model.FriendInvites.Remove(characterFriendInvite.Model);

            _log.LogTrace("Removed friend invite {InviteId} from character {Identity}.", invite.Id, Identity);

            await _messagePublisher.PublishAsync(new FriendshipInviteRemovedMessage
            {
                FriendInvite = await invite.ToInternalFriendInvite()
            });
        }

        /// <summary>
        /// Return the pending <see cref="FriendInvite"/> for the specified character identity.
        /// </summary>
        /// <param name="identity">The identity of the character for which to retrieve the pending friend invite.</param>
        public async Task<FriendInvite> GetFriendInvitePendingAsync(Identity identity)
        {
            foreach (CharacterFriendInvitePending pendingInvite in _invitesPending.Values)
            {
                FriendInvite invite = await pendingInvite.GetFriendInviteAsync();
                if (invite.InviteeCharacterIdentity == identity)
                    return invite;
            }

            return null;
        }

        /// <summary>
        /// Add a new pending <see cref="FriendInvite"/> to the <see cref="Character"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="FriendInvite"/> should have a valid id, this should come from the database.
        /// The use of a transaction should be used to ensure the invite is created and added to the character atomically.
        /// </remarks>
        /// <param name="invite">Invite to add to the <see cref="Character"/>.</param>
        /// <exception cref="ArgumentException">Thrown if the invite does not have a valid identifier.</exception>
        public void AddFriendInvitePending(FriendInvite invite)
        {
            if (invite.Id == 0)
                throw new ArgumentException("Invite must have a valid id.", nameof(invite));

            var characterFriendInvitePending = _serviceProvider.GetRequiredService<CharacterFriendInvitePending>();
            characterFriendInvitePending.Initialise(invite.Id);

            Model.FriendInvitesPending.Add(characterFriendInvitePending.Model);
            _invitesPending.Add(invite.Id, characterFriendInvitePending);

            _log.LogTrace("Added pending friend invite {InviteId} to character {Identity}.", invite.Id, Identity);
        }

        /// <summary>
        /// Remove a pending <see cref="FriendInvite"/> from the <see cref="Character"/>.
        /// </summary>
        /// <param name="invite">Invite pending to remove from the <see cref="Character"/>.</param>
        public void RemoveFriendInvitePending(FriendInvite invite)
        {
            if (!_invitesPending.TryGetValue(invite.Id, out CharacterFriendInvitePending characterFriendInvitePending))
                return;

            Model.FriendInvitesPending.Remove(characterFriendInvitePending.Model);
            _invitesPending.Remove(invite.Id);

            _log.LogTrace("Removed pending friend invite {InviteId} from character {Identity}.", invite.Id, Identity);
        }

        /// <summary>
        /// Send the list of friends for the <see cref="Character"/>.
        /// </summary>
        public async Task SendFriendsAsync()
        {
            var friendshipList = new FriendshipListMessage
            {
                Character = this.ToInternalCharacter()
            };

            foreach (CharacterFriend characterFriend in _friends.Values)
            {
                Friend.Friend friend = await characterFriend.GetFriendAsync();
                if (friend != null)
                    friendshipList.Friends.Add(await friend.ToInternalFriendAsync());
            }

            await _messagePublisher.PublishAsync(friendshipList);
        }

        /// <summary>
        /// Get the number of friends for the <see cref="Character"/>.
        /// </summary>
        /// <param name="type">The <see cref="FriendshipType"/> of friendships to count.</param>
        /// <returns>The number of friends of the specified type.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the type is <see cref="FriendshipType.Account"/>. Use <see cref="Account.Account.GetFriendCount"/> instead.</exception>
        public async Task<uint> GetFriendCountAsync(FriendshipType type)
        {
            if (type == FriendshipType.Account)
                throw new ArgumentOutOfRangeException(nameof(type), "Character cannot have account type friendships.");

            uint count = 0;
            foreach (CharacterFriend characterFriend in _friends.Values)
            {
                Friend.Friend friend = await characterFriend.GetFriendAsync();
                if (friend.Type == type)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Return a <see cref="Friend.Friend"/> that belongs to the <see cref="Character"/> with the specified friend id.
        /// </summary>
        /// <param name="friendId">Id of the friend to retrieve.</param>
        public async Task<Friend.Friend> GetFriendAsync(ulong friendId)
        {
            if (!_friends.TryGetValue(friendId, out CharacterFriend characterFriend))
                return null;

            return await characterFriend.GetFriendAsync();
        }

        /// <summary>
        /// Return a <see cref="Friend.Friend"/> that belongs to the <see cref="Character"/> with the specified identity.
        /// </summary>
        /// <param name="identity">Identity of the character to retrieve the friend for.</param>
        public async Task<Friend.Friend> GetFriendByIdentityAsync(Identity identity)
        {
            foreach (CharacterFriend item in _friends.Values)
            {
                Friend.Friend friend = await item.GetFriendAsync();
                if (friend.InviteeIdentity == identity)
                    return friend;
            }

            return null;
        }

        /// <summary>
        /// Return a collection of other characters that have added the <see cref="Character"/> as a friend.
        /// </summary>
        public async Task<IEnumerable<Friend.Friend>> GetFriendsInverseAsync()
        {
            var list = new List<Friend.Friend>();
            foreach (CharacterFriendInverse characterFriendInverse in _friendsInverse.Values)
                list.Add(await characterFriendInverse.GetFriendAsync());

            return list;
        }

        /// <summary>
        /// Add a new <see cref="Friend.Friend"/> to the <see cref="Character"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="Friend.Friend"/> should have a valid id, this should come from the database.
        /// The use of a transaction should be used to ensure the friend is created and added to the character atomically.
        /// </remarks>
        /// <param name="friend">Friend to add to the <see cref="Character"/>.</param>
        /// <exception cref="ArgumentException">Thrown if the friend does not have a valid identifier.</exception>
        public async Task AddFriendAsync(Friend.Friend friend)
        {
            if (friend.Id == 0)
                throw new ArgumentException("Friend must have a valid id.", nameof(friend));

            var characterFriend = _serviceProvider.GetRequiredService<CharacterFriend>();
            characterFriend.Initialise(friend.Id);

            Model.Friends.Add(characterFriend.Model);
            _friends.Add(characterFriend.FriendId, characterFriend);

            _log.LogTrace("Added friend {FriendId} to character {Identity}.", friend.Id, Identity);

            await _messagePublisher.PublishAsync(new FriendshipAddedMessage
            {
                Friend = await friend.ToInternalFriendAsync()
            });
        }

        /// <summary>
        /// Remove an existing <see cref="Friend.Friend"/> from the <see cref="Character"/>.
        /// </summary>
        /// <param name="friend">Friend to remove from the <see cref="Character"/>.</param>
        public async Task RemoveFriendAsync(Friend.Friend friend)
        {
            if (!_friends.Remove(friend.Id, out CharacterFriend characterFriend))
                return;

            Model.Friends.Remove(characterFriend.Model);

            _log.LogTrace("Removed friend {FriendId} from character {Identity}.", friend.Id, Identity);

            await _messagePublisher.PublishAsync(new FriendshipRemovedMessage
            {
                Friend = await friend.ToInternalFriendAsync()
            });
        }

        /// <summary>
        /// Add a new <see cref="Friend.Friend"/> to the <see cref="Character"/> as an inverse friend.
        /// </summary>
        /// <remarks>
        /// An inverse friend represents another character that has added this character as a friend.
        /// </remarks>
        /// <param name="friend">Friend to add as an inverse friend.</param>
        /// <exception cref="ArgumentException">Thrown if the friend does not have a valid identifier.</exception>
        public void AddFriendInverse(Friend.Friend friend)
        {
            if (friend.Id == 0)
                throw new ArgumentException("Friend must have a valid id.", nameof(friend));

            var characterFriendInverse = _serviceProvider.GetRequiredService<CharacterFriendInverse>();
            characterFriendInverse.Initialise(friend.Id);

            Model.FriendsInverse.Add(characterFriendInverse.Model);
            _friendsInverse.Add(characterFriendInverse.FriendId, characterFriendInverse);

            _log.LogTrace("Added inverse friend {FriendId} to character {Identity}.", friend.Id, Identity);
        }

        /// <summary>
        /// Remove an existing inverse <see cref="Friend.Friend"/> from the <see cref="Character"/>.
        /// </summary>
        /// <remarks>
        /// An inverse friend represents another character that has added this character as a friend.
        /// </remarks>
        /// <param name="friend">Friend to remove as an inverse friend.</param>
        public void RemoveFriendInverse(Friend.Friend friend)
        {
            if (!_friendsInverse.Remove(friend.Id, out CharacterFriendInverse characterFriendInverse))
                return;

            Model.FriendsInverse.Remove(characterFriendInverse.Model);

            _log.LogTrace("Removed inverse friend {FriendId} from character {Identity}.", friend.Id, Identity);
        }

        /// <summary>
        /// Get the value of a <see cref="Stat"/> for <see cref="Character"/>.
        /// </summary>
        /// <param name="stat">Stat to get the value.</param>
        /// <returns>The value of the specified stat, or null if the stat is not set.</returns>
        public float? GetStat(Stat stat)
        {
            if (!_stats.TryGetValue(stat, out CharacterStat characterStat))
                return null;

            return characterStat.Value;
        }

        /// <summary>
        /// Set the value of a <see cref="Stat"/> for <see cref="Character"/>.
        /// </summary>
        /// <param name="stat">Stat to set the value.</param>
        /// <param name="value">Value to assign to the stat.</param>
        public void SetStat(Stat stat, float value)
        {
            if (!_stats.TryGetValue(stat, out CharacterStat characterStat))
            {
                characterStat = _serviceProvider.GetRequiredService<CharacterStat>();
                characterStat.Initialise(stat, value);

                Model.Stats.Add(characterStat.Model);
                _stats.Add(stat, characterStat);
            }

            characterStat.Value = value;

            _log.LogTrace("Set stat {Stat} to {Value} for character {Identity}.", stat, value, Identity);
        }
    }
}
