using NexusForever.Database.Friendship;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Server.Friendship.Game.Account;
using NexusForever.Server.Friendship.Game.Friend;
using Rebus.Handlers;

namespace NexusForever.Server.Friendship.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountRemoveHandler : IHandleMessages<FriendshipAccountRemoveMessage>
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly AccountManager _accountManager;
        private readonly FriendAccountManager _friendManager;

        public FriendshipAccountRemoveHandler(
            FriendshipContext context,
            AccountManager accountManager,
            FriendAccountManager friendManager)
        {
            _context        = context;
            _accountManager = accountManager;
            _friendManager  = friendManager;
        }

        #endregion

        public async Task Handle(FriendshipAccountRemoveMessage message)
        {
            Account inviterAccount = await _accountManager.GetAccountAsync(message.AccountId);
            if (inviterAccount == null)
                return;

            FriendAccount inviterFriend = await inviterAccount.GetFriendAsync(message.AccountFriendId);
            if (inviterFriend == null)
                return;

            Account inviteeAccount = await inviterFriend.GetInviteeAccountAsync();
            if (inviteeAccount == null)
                return;

            FriendAccount inviteeFriend = await inviteeAccount.GetFriendByAccountId(inviterAccount.Id);
            if (inviteeFriend == null)
                return;

            await inviterAccount.RemoveFriendAsync(inviterFriend);
            inviteeAccount.RemoveFriendInverse(inviterFriend);
            _friendManager.RemoveFriend(inviterFriend);

            await inviteeAccount.RemoveFriendAsync(inviteeFriend);
            inviterAccount.RemoveFriendInverse(inviteeFriend);
            _friendManager.RemoveFriend(inviteeFriend);

            await _context.SaveChangesAsync();
        }
    }
}
