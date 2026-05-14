using NexusForever.Database.Friendship;
using NexusForever.Game.Static.Friendship;
using NexusForever.GameTable.Text.Filter;
using NexusForever.GameTable.Text.Static;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Server.Friendship.Game.Character;
using NexusForever.Server.Friendship.Game.Friend;
using Rebus.Handlers;

namespace NexusForever.Server.Friendship.Network.Internal.Handler.Friendship
{
    public class FriendshipNoteUpdateByIdentityHandler : IHandleMessages<FriendshipNoteUpdateByIdentityMessage>
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;

        private readonly CharacterManager _characterManager;
        private readonly ITextFilterManager _textFilterManager;
        private readonly FriendshipResultPublisher _friendshipResultPublisher;

        public FriendshipNoteUpdateByIdentityHandler(
            FriendshipContext context,
            OutboxMessagePublisher messagePublisher,
            CharacterManager characterManager,
            ITextFilterManager textFilterManager,
            FriendshipResultPublisher friendshipResultPublisher)
        {
            _context                   = context;
            _messagePublisher          = messagePublisher;
            _characterManager          = characterManager;
            _textFilterManager         = textFilterManager;
            _friendshipResultPublisher = friendshipResultPublisher;
        }

        #endregion

        public async Task Handle(FriendshipNoteUpdateByIdentityMessage message)
        {
            Task<FriendshipResult?> task = UpdateNoteAsync(message);
            await _friendshipResultPublisher.PublishResultAsync(_messagePublisher, message.Source, task);

            await _context.SaveChangesAsync();
        }

        private async Task<FriendshipResult?> UpdateNoteAsync(FriendshipNoteUpdateByIdentityMessage message)
        {
            Character character = await _characterManager.GetCharacterAsync(message.Source.ToFriendshipIdentity());
            if (character == null)
                return FriendshipResult.PlayerNotFound;

            Friend friend = await character.GetFriendByIdentityAsync(message.Target.ToFriendshipIdentity());
            if (friend == null)
                return FriendshipResult.PlayerNotFriend;

            if (message.Note != null)
            {
                if (!_textFilterManager.IsTextValid(message.Note, UserText.FriendshipNote))
                    return FriendshipResult.InvalidDisplayName;

                if (!_textFilterManager.IsTextValid(message.Note))
                    return FriendshipResult.ContainsProfanity;
            }

            await friend.UpdateNoteAsync(message.Note);

            return null;
        }
    }
}
