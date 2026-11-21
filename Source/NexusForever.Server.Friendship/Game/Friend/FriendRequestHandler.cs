using NexusForever.Game.Static.Friendship;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public class FriendRequestHandler
    {
        #region Dependency Injection

        private readonly FriendFactory _friendFactory;
        private readonly FriendInviteFactory _inviteFactory;
        private readonly FriendAccountInviteFactory _accountInviteFactory;

        public FriendRequestHandler(
            FriendFactory friendFactory,
            FriendInviteFactory inviteFactory,
            FriendAccountInviteFactory accountInviteFactory)
        {
            _friendFactory        = friendFactory;
            _inviteFactory        = inviteFactory;
            _accountInviteFactory = accountInviteFactory;
        }

        #endregion

        public async Task<FriendshipResult?> HandleRequestAsync(Character.Character inviter, Character.Character invitee, FriendshipType type, string note)
        {
            switch (type)
            {
                case FriendshipType.Friend:
                    _ = await _inviteFactory.CreateFriendInviteAsync(inviter, invitee, note);
                    break;
                case FriendshipType.Ignore:
                {
                    Friend friend = await inviter.GetFriendByIdentityAsync(invitee.Identity);
                    if (friend != null)
                        await friend.UpdateType(FriendshipType.Ignore);
                    else
                        _ = await _friendFactory.CreateFriendAsync(inviter, invitee, FriendshipType.Ignore);

                    break;
                }
                case FriendshipType.Rival:
                {
                    Friend friend = await inviter.GetFriendByIdentityAsync(invitee.Identity);
                    if (friend?.Type == FriendshipType.Friend)
                        await friend.UpdateType(FriendshipType.FriendAndRival);
                    else
                        _ = await _friendFactory.CreateFriendAsync(inviter, invitee, FriendshipType.Rival);

                    break;
                }
                case FriendshipType.Account:
                {
                    Account.Account inviterAccount = await inviter.GetAccountAsync();
                    if (inviterAccount == null)
                        return FriendshipResult.PlayerNotFound;

                    Account.Account inviteeAccount = await invitee.GetAccountAsync();
                    if (inviteeAccount == null)
                        return FriendshipResult.PlayerNotFound;

                    _ = await _accountInviteFactory.CreateFriendInviteAsync(inviterAccount, inviteeAccount, note);
                    break;
                }
                default:
                    return FriendshipResult.InvalidType;
            }

            return null;
        }
    }
}
