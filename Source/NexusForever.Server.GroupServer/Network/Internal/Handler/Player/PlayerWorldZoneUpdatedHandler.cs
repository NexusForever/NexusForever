using NexusForever.Database.Group;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.GroupServer.Character;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Player
{
    public class PlayerWorldZoneUpdatedHandler : IHandleMessages<PlayerWorldZoneUpdatedMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _context;
        private readonly CharacterManager _characterManager;

        public PlayerWorldZoneUpdatedHandler(
            GroupContext context,
            CharacterManager characterManager)
        {
            _context          = context;
            _characterManager = characterManager;
        }

        #endregion

        public async Task Handle(PlayerWorldZoneUpdatedMessage message)
        {
            Character.Character character = await _characterManager.GetCharacterAsync(message.Identity.ToGroupIdentity());
            if (character == null)
                return;

            character.WorldZoneId = message.WorldZoneId;
            character.RealmDirty  = true;

            await _context.SaveChangesAsync();
        }
    }
}
