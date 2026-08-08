using Microsoft.Extensions.Options;
using NexusForever.Game.Static.Friendship;
using NexusForever.GameTable.Text.Filter;
using NexusForever.GameTable.Text.Static;
using NexusForever.Server.Friendship.Configuration;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public class FriendInviteValidator
    {
        #region Dependency Injection

        private readonly LimitOptions _limitOptions;
        private readonly ITextFilterManager _textFilterManager;
        private readonly FriendAccountInviteValidator _accountInviteValidator;

        public FriendInviteValidator(
            IOptions<LimitOptions> limitOptions,
            ITextFilterManager textFilterManager,
            FriendAccountInviteValidator accountInviteValidator)
        {
            _limitOptions           = limitOptions.Value;
            _textFilterManager      = textFilterManager;
            _accountInviteValidator = accountInviteValidator;
        }

        #endregion

        /// <summary>
        /// Validates a friend invite request.
        /// </summary>
        /// <param name="inviter">Character initiating the friend invite.</param>
        /// <param name="invitee">Character receiving the friend invite.</param>
        /// <param name="type">Friend invite type.</param>
        /// <param name="note">Optional note for the friend invite.</param>
        /// <returns>Returns a <see cref="FriendshipResult"/> if the request is invalid, or null if the request is valid.</returns>
        public async Task<FriendshipResult?> ValidatorAsync(Character.Character inviter, Character.Character invitee, FriendshipType type, string note)
        {
            if (type == FriendshipType.Account)
            {
                Account.Account inviterAccount = await inviter.GetAccountAsync();
                if (inviterAccount == null)
                    return FriendshipResult.PlayerNotFound;

                Account.Account inviteeAccount = await invitee.GetAccountAsync();
                if (inviteeAccount == null)
                    return FriendshipResult.PlayerNotFound;

                return await _accountInviteValidator.ValidateAsync(inviterAccount, inviteeAccount, note);
            }
            else
            {
                if (invitee.Identity == inviter.Identity)
                    return FriendshipResult.CannotInviteSelf;

                if (invitee.Faction != inviter.Faction)
                    return FriendshipResult.RequestDenied;

                Friend friend = await inviter.GetFriendByIdentityAsync(invitee.Identity);
                if (friend != null)
                {
                    if (friend.Type == FriendshipType.Ignore && type != FriendshipType.Ignore)
                        return FriendshipResult.PlayerOnIgnored;

                    if (friend.Type == type)
                    {
                        switch (type)
                        {
                            case FriendshipType.Friend:
                                return FriendshipResult.PlayerAlreadyFriend;
                            case FriendshipType.Ignore:
                                return FriendshipResult.PlayerAlreadyIgnored;
                            case FriendshipType.Rival:
                                return FriendshipResult.PlayerAlreadyRival;
                        }
                    }

                    if (friend.Type == FriendshipType.FriendAndRival)
                    {
                        switch (type)
                        {
                            case FriendshipType.Friend:
                                return FriendshipResult.PlayerAlreadyFriend;
                            case FriendshipType.Rival:
                                return FriendshipResult.PlayerAlreadyRival;
                        }
                    }
                }

                uint friendCount = await inviter.GetFriendCountAsync(type);
                switch (type)
                {
                    case FriendshipType.Friend:
                        if (friendCount >= _limitOptions.MaxFriends)
                            return FriendshipResult.MaxFriends;
                        break;
                    case FriendshipType.Ignore:
                        if (friendCount >= _limitOptions.MaxIgnored)
                            return FriendshipResult.MaxIgnored;
                        break;
                    case FriendshipType.Rival:
                        if (friendCount >= _limitOptions.MaxRivals)
                            return FriendshipResult.MaxRivals;
                        break;
                    
                }

                if (invitee.GetFriendInviteCount() >= _limitOptions.MaxFriendRequests)
                    return FriendshipResult.PlayerQueuedRequests;

                if (note != null)
                {
                    if (!_textFilterManager.IsTextValid(note, UserText.FriendshipInviteNote))
                        return FriendshipResult.InvalidInviteNote;

                    if (!_textFilterManager.IsTextValid(note))
                        return FriendshipResult.ContainsProfanity;
                }
            }

            // PlayerOffline - do we care?
            // FriendsBlocked

            return null;
        }
    }
}
