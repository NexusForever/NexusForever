using NexusForever.Database.Group;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.GroupServer.Character;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Player
{
    public class PlayerWorldUpdatedHandler : IHandleMessages<PlayerWorldUpdatedMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _context;
        private readonly CharacterManager _characterManager;

        public PlayerWorldUpdatedHandler(
            GroupContext context,
            CharacterManager characterManager)
        {
            _context          = context;
            _characterManager = characterManager;
        }

        #endregion

        public async Task Handle(PlayerWorldUpdatedMessage message)
        {
            Character.Character character = await _characterManager.GetCharacterAsync(message.Identity.ToGroupIdentity());
            if (character == null)
                return;

            character.WorldId    = message.WorldId;
            character.RealmDirty = true;

            await _context.SaveChangesAsync();
        }
    }
}
