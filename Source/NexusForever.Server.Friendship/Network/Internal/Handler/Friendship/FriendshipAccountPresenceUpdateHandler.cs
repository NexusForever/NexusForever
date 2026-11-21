using NexusForever.Database.Friendship;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Server.Friendship.Game.Account;
using Rebus.Handlers;

namespace NexusForever.Server.Friendship.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountPresenceUpdateHandler : IHandleMessages<FriendshipAccountPresenceUpdateMessage>
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly AccountManager _accountManager;

        public FriendshipAccountPresenceUpdateHandler(
            FriendshipContext context,
            AccountManager accountManager)
        {
            _context        = context;
            _accountManager = accountManager;
        }

        #endregion

        public async Task Handle(FriendshipAccountPresenceUpdateMessage message)
        {
            Account account = await _accountManager.GetAccountAsync(message.AccountId);
            if (account == null)
                return;

            await account.SetPresenceAsync(message.Presence);

            await _context.SaveChangesAsync();
        }
    }
}
