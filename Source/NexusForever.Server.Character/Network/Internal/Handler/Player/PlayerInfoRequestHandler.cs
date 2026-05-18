using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.Character.Game.Character;
using Rebus.Handlers;

namespace NexusForever.Server.Character.Network.Internal.Handler.Player
{
    public class PlayerInfoRequestHandler : IHandleMessages<PlayerInfoRequestMessage>
    {
        #region Dependency Injection

        private readonly CharacterManager _characterManager;
        private readonly IInternalMessagePublisher _messagePublisher;

        public PlayerInfoRequestHandler(
            CharacterManager characterManager,
            IInternalMessagePublisher messagePublisher)
        {
            _characterManager = characterManager;
            _messagePublisher = messagePublisher;
        }

        #endregion

        public async Task Handle(PlayerInfoRequestMessage message)
        {
            var playerInfoResponse = new PlayerInfoResponseMessage
            {
                Source = message.Source,
                Target = message.Target,
                Type   = message.Type
            };

            Game.Character.Character character = await _characterManager.GetCharacterRemoteAsync(message.Target.ToQueryIdentity());
            if (character != null)
            {
                playerInfoResponse.PlayerInfo = new PlayerInfo
                {
                    IdentityName = character.IdentityName.ToInternalIdentity(),
                    Class        = character.Class,
                    Path         = character.Path,
                    Faction      = character.Faction,
                    Level        = (byte)character.Level,
                    LastOnline   = character.LastOnline
                };
            }

            await _messagePublisher.PublishAsync(playerInfoResponse);
        }
    }
}
