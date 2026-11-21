using NexusForever.Database.Query;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.Character.Game.Character;
using Rebus.Handlers;

namespace NexusForever.Server.Character.Network.Internal.Handler.Player
{
    public class PlayerLoggedInHandler : IHandleMessages<PlayerLoggedInMessage>
    {
        #region Dependency Injection

        private readonly QueryContext _context;
        private readonly CharacterManager _characterManager;

        public PlayerLoggedInHandler(
            QueryContext context,
            CharacterManager characterManager)
        {
            _context          = context;
            _characterManager = characterManager;
        }

        #endregion

        public async Task Handle(PlayerLoggedInMessage message)
        {
            var character = await _characterManager.GetCharacterRemoteAsync(message.Identity.ToQueryIdentity());
            if (character == null)
                return;

            character.LastOnline = null;

            await _context.SaveChangesAsync();
        }
    }
}
