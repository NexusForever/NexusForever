using NexusForever.Database.Query;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.Character.Game.Character;
using Rebus.Handlers;

namespace NexusForever.Server.Character.Network.Internal.Handler.Player
{
    public class PlayerStatUpdatedHandler : IHandleMessages<PlayerStatUpdatedMessage>
    {
        #region Dependency Injection

        private readonly QueryContext _context;
        private readonly CharacterManager _characterManager;

        public PlayerStatUpdatedHandler(
            QueryContext context,
            CharacterManager characterManager)
        {
            _context          = context;
            _characterManager = characterManager;
        }

        #endregion

        public async Task Handle(PlayerStatUpdatedMessage message)
        {
            // we don't care about any stat updates except level
            if (message.Stat != Stat.Level)
                return;

            var character = await _characterManager.GetCharacterRemoteAsync(message.Identity.ToQueryIdentity());
            if (character == null)
                return;

            character.Model.Level = (uint)message.Value;

            await _context.SaveChangesAsync();
        }
    }
}
