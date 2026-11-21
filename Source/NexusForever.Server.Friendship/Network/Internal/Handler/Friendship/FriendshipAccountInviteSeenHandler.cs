using NexusForever.Database.Friendship;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Server.Friendship.Game.Account;
using NexusForever.Server.Friendship.Game.Friend;
using Rebus.Handlers;

namespace NexusForever.Server.Friendship.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountInviteSeenHandler : IHandleMessages<FriendshipAccountInviteMarkSeenMessage>
    {
        #region Dependency Injection

        private readonly AccountManager _accountManager;
        private readonly FriendshipContext _friendContext;

        public FriendshipAccountInviteSeenHandler(
            AccountManager accountManager,
            FriendshipContext friendContext)
        {
            _accountManager = accountManager;
            _friendContext  = friendContext;
        }

        #endregion

        public async Task Handle(FriendshipAccountInviteMarkSeenMessage message)
        {
            Account account = await _accountManager.GetAccountAsync(message.AccountId);
            if (account == null)
                return;

            FriendAccountInvite invite = await account.GetFriendInviteAsync(message.AccountFriendInviteId);
            if (invite == null)
                return;

            invite.Seen = true;

            // is this required?
            await account.SendFriendInvitesAsync();

            await _friendContext.SaveChangesAsync();
        }
    }
}
