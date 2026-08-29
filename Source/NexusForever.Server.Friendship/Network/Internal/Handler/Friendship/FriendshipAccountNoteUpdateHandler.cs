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
    public class FriendshipAccountNoteUpdateHandler : IHandleMessages<FriendshipAccountNoteUpdateMessage>
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;

        private readonly AccountManager _accountManager;
        private readonly ITextFilterManager _textFilterManager;
        private readonly FriendshipResultPublisher _friendshipResultPublisher;

        public FriendshipAccountNoteUpdateHandler(
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

        public async Task Handle(FriendshipAccountNoteUpdateMessage message)
        {
            Task<FriendshipResult?> task = NoteUpdateAsync(message);
            await _friendshipResultPublisher.PublishResultAsync(_messagePublisher, message.Identity, task);

            await _context.SaveChangesAsync();
        }

        private async Task<FriendshipResult?> NoteUpdateAsync(FriendshipAccountNoteUpdateMessage message)
        {
            Account account = await _accountManager.GetAccountAsync(message.AccountId);
            if (account == null)
                return FriendshipResult.FriendshipNotFound;

            FriendAccount friend = await account.GetFriendAsync(message.FriendAccountId);
            if (friend == null)
                return FriendshipResult.FriendshipNotFound;

            if (!_textFilterManager.IsTextValid(message.Note))
                return FriendshipResult.ContainsProfanity;

            if (!_textFilterManager.IsTextValid(message.Note, UserText.FriendshipAccountPrivateNote))
                return FriendshipResult.InvalidNote;

            await friend.UpdateNoteAsync(message.Note);

            return null;
        }
    }
}
