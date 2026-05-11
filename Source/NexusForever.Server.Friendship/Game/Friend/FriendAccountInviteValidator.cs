using Microsoft.Extensions.Options;
using NexusForever.Game.Static.Friendship;
using NexusForever.GameTable.Text.Filter;
using NexusForever.GameTable.Text.Static;
using NexusForever.Server.Friendship.Configuration;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public class FriendAccountInviteValidator
    {
        #region Dependency Injection

        private readonly LimitOptions _limitOptions;
        private readonly ITextFilterManager _textFilterManager;

        public FriendAccountInviteValidator(
            IOptions<LimitOptions> limitOptions,
            ITextFilterManager textFilterManager)
        {
            _limitOptions      = limitOptions.Value;
            _textFilterManager = textFilterManager;
        }

        #endregion

        /// <summary>
        /// Validates an account friend invite request.
        /// </summary>
        /// <param name="inviter">Account initiating the friend invite.</param>
        /// <param name="invitee">Account receiving the friend invite.</param>
        /// <param name="note">Optional note for the friend invite.</param>
        /// <returns>Returns a <see cref="FriendshipResult"/> if the request is invalid, or null if the request is valid.</returns>
        public async Task<FriendshipResult?> ValidateAsync(Account.Account inviter, Account.Account invitee, string note)
        {
            if (inviter.InvitePrivilegesSuspended)
                return FriendshipResult.PrivilegesSuspended;

            if (invitee.BlockAccountFriendRequests)
                return FriendshipResult.BlockedForStrangers;

            if (inviter.Id == invitee.Id)
                return FriendshipResult.CannotInviteSelf;

            if (inviter.GetFriendCount() >= _limitOptions.MaxAccountFriends)
                return FriendshipResult.MaxFriends;

            if (invitee.GetFriendInviteCount() >= _limitOptions.MaxAccountFriendRequests)
                return FriendshipResult.PlayerQueuedRequests;

            if (await inviter.GetFriendInvitePendingAsync(invitee.Id) != null)
                return FriendshipResult.PlayerQueuedRequests;

            if (await inviter.GetFriendByAccountId(invitee.Id) != null)
                return FriendshipResult.PlayerAlreadyFriend;

            if (note != null)
            {
                if (!_textFilterManager.IsTextValid(note, UserText.FriendshipInviteNote))
                    return FriendshipResult.InvalidInviteNote;

                if (!_textFilterManager.IsTextValid(note))
                    return FriendshipResult.ContainsProfanity;
            }

            return null;
        }
    }
}
