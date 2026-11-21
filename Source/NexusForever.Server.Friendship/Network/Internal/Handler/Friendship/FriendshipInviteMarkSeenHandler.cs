using NexusForever.Database.Friendship;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Server.Friendship.Game.Character;
using NexusForever.Server.Friendship.Game.Friend;
using Rebus.Handlers;

namespace NexusForever.Server.Friendship.Network.Internal.Handler.Friendship
{
    public class FriendshipInviteMarkSeenHandler : IHandleMessages<FriendshipInviteMarkSeenMessage>
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly CharacterManager _characterManager;

        public FriendshipInviteMarkSeenHandler(
            FriendshipContext context,
            CharacterManager characterManager)
        {
            _context          = context;
            _characterManager = characterManager;
        }

        #endregion

        public async Task Handle(FriendshipInviteMarkSeenMessage message)
        {
            Character character = await _characterManager.GetCharacterAsync(message.Identity.ToFriendshipIdentity());
            if (character == null)
                return;

            foreach (ulong friendInviteId in message.FriendInviteIds)
            {
                FriendInvite invite = await character.GetFriendInviteAsync(friendInviteId);
                if (invite == null)
                    continue;

                invite.Seen = true;
            }

            await character.SendFriendInvitesAsync();

            await _context.SaveChangesAsync();
        }
    }
}
