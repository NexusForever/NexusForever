using NexusForever.API.Character.Client;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Player;
using Rebus.Handlers;

namespace NexusForever.Server.Character.Network.Internal.Handler.Player
{
    public class PlayerInfoRequestHandler : IHandleMessages<PlayerInfoRequestMessage>
    {
        #region Dependency Injection

        private readonly CharacterAPIClient _characterAPIClient;
        private readonly IInternalMessagePublisher _messagePublisher;

        public PlayerInfoRequestHandler(
            CharacterAPIClient characterAPIClient,
            IInternalMessagePublisher messagePublisher)
        {
            _characterAPIClient = characterAPIClient;
            _messagePublisher   = messagePublisher;
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

            API.Model.Character.Character character = await _characterAPIClient.GetCharacterAsync(message.Target.ToAPIdentity());
            if (character != null)
            {
                playerInfoResponse.PlayerInfo = new PlayerInfo
                {
                    IdentityName = character.IdentityName.ToInternalIdentity(),
                    Class        = character.Class,
                    Path         = character.Path,
                    Faction      = character.Faction,
                    Level        = (byte)(character.Stats.SingleOrDefault(s => s.Stat == Stat.Level)?.Value ?? 0),
                    LastOnline   = !character.IsOnline ? character.LastOnline : null
                };
            }

            await _messagePublisher.PublishAsync(playerInfoResponse);
        }
    }
}
