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
    public class FriendshipAccountNicknameUpdateHandler : IHandleMessages<FriendshipAccountNicknameUpdateMessage>
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;

        private readonly AccountManager _accountManager;
        private readonly ITextFilterManager _textFilterManager;
        private readonly FriendshipResultPublisher _friendshipResultPublisher;

        public FriendshipAccountNicknameUpdateHandler(
            FriendshipContext context,
            OutboxMessagePublisher messagePublisher,
            AccountManager accountManager,
            ITextFilterManager textFilterManager,
            FriendshipResultPublisher friendshipResultPublisher)
        {
            _context                   = context;
            _messagePublisher          = messagePublisher;
            _accountManager            = accountManager;
            _textFilterManager         = textFilterManager;
            _friendshipResultPublisher = friendshipResultPublisher;
        }

        #endregion

        public async Task Handle(FriendshipAccountNicknameUpdateMessage message)
        {
            Task<FriendshipResult?> task = AccountSetNicknameAsync(message);
            await _friendshipResultPublisher.PublishResultAsync(_messagePublisher, message.Source, task);

            await _context.SaveChangesAsync();
        }

        private async Task<FriendshipResult?> AccountSetNicknameAsync(FriendshipAccountNicknameUpdateMessage message)
        {
            Account account = await _accountManager.GetAccountAsync(message.AccountId);
            if (account == null)
                return FriendshipResult.PlayerNotFound;

            if (message.AccountNickname != null)
            {
                if (!_textFilterManager.IsTextValid(message.AccountNickname, UserText.FriendshipAccountName))
                    return FriendshipResult.InvalidDisplayName;

                if (!_textFilterManager.IsTextValid(message.AccountNickname))
                    return FriendshipResult.ContainsProfanity;

                if (await _accountManager.GetAccountByNickname(message.AccountNickname) != null)
                    return FriendshipResult.NameUnavailable;
            }

            await account.SetNicknameAsync(message.AccountNickname);

            return null;
        }
    }
}
