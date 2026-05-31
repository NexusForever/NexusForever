using NexusForever.Database.Friendship;
using NexusForever.Game.Static.Friendship;
using NexusForever.GameTable.Text.Filter;
using NexusForever.GameTable.Text.Static;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Server.Friendship.Game.Account;
using NexusForever.Server.Friendship.Game.Friend;
using Rebus.Handlers;

namespace NexusForever.Server.Friendship.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountStatusUpdateHandler : IHandleMessages<FriendshipAccountStatusUpdateMessage>
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;

        private readonly AccountManager _accountManager;
        private readonly ITextFilterManager _textFilterManager;
        private readonly FriendshipResultPublisher _friendshipResultPublisher;

        public FriendshipAccountStatusUpdateHandler(
            FriendshipContext context,
            OutboxMessagePublisher messagePublisher,
            AccountManager accountManager,
            ITextFilterManager textFilterManager,
            FriendshipResultPublisher friendshipResultPublisher)
        {
            _context                   = context;
            _accountManager            = accountManager;
            _textFilterManager         = textFilterManager;
            _messagePublisher          = messagePublisher;
            _friendshipResultPublisher = friendshipResultPublisher;
        }

        #endregion

        public async Task Handle(FriendshipAccountStatusUpdateMessage message)
        {
            Task<FriendshipResult?> task = SetAccountStatus(message);
            await _friendshipResultPublisher.PublishResultAsync(_messagePublisher, message.Identity, task);

            await _context.SaveChangesAsync();
        }

        private async Task<FriendshipResult?> SetAccountStatus(FriendshipAccountStatusUpdateMessage message)
        {
            Account account = await _accountManager.GetAccountAsync(message.AccountId);
            if (account == null)
                return FriendshipResult.FriendshipNotFound;

            if (message.Status != null)
            {
                if (!_textFilterManager.IsTextValid(message.Status))
                    return FriendshipResult.ContainsProfanity;

                if (!_textFilterManager.IsTextValid(message.Status, UserText.FriendshipAccountPublicNote))
                    return FriendshipResult.InvalidPublicNote;
            }

            await account.SetStatusAsync(message.Status);

            return null;
        }
    }
}
