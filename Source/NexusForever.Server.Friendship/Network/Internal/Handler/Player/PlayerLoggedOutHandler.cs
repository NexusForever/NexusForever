using NexusForever.Database.Friendship;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.Friendship.Game.Account;
using NexusForever.Server.Friendship.Game.Character;
using Rebus.Handlers;

namespace NexusForever.Server.Friendship.Network.Internal.Handler.Player
{
    public class PlayerLoggedOutHandler : IHandleMessages<PlayerLoggedOutMessage>
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly CharacterManager _characterManager;

        public PlayerLoggedOutHandler(
            FriendshipContext context,
            CharacterManager characterManager)
        {
            _context          = context;
            _characterManager = characterManager;
        }

        #endregion

        public async Task Handle(PlayerLoggedOutMessage message)
        {
            Character character = await _characterManager.GetCharacterAsync(message.Identity.ToFriendshipIdentity());
            if (character == null)
                return;

            await character.SetLastOnline(DateTime.UtcNow);

            Account account = await character.GetAccountAsync();
            if (account == null)
                return;

            await account.SetLastOnline(DateTime.UtcNow);
            await account.SetActiveCharacterAsync(null);

            await _context.SaveChangesAsync();
        }
    }
}
