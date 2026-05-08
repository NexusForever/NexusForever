using NexusForever.Database.Friendship;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.Friendship.Game.Character;
using Rebus.Handlers;

namespace NexusForever.Server.Friendship.Network.Internal.Handler.Player
{
    public class PlayerWorldZoneUpdatedHandler : IHandleMessages<PlayerWorldZoneUpdatedMessage>
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly CharacterManager _characterManager;

        public PlayerWorldZoneUpdatedHandler(
            FriendshipContext context,
            CharacterManager characterManager)
        {
            _context          = context;
            _characterManager = characterManager;
        }

        #endregion

        public async Task Handle(PlayerWorldZoneUpdatedMessage message)
        {
            Character character = await _characterManager.GetCharacterAsync(message.Identity.ToFriendshipIdentity());
            if (character == null)
                return;

            character.WorldZoneId = message.WorldZoneId;

            await _context.SaveChangesAsync();
        }
    }
}
