using System.Threading.RateLimiting;
using NexusForever.Database.Query.Repository.Query;
using NexusForever.Game.Static.Who;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Who;
using NexusForever.Server.Character.Game.Character;
using NexusForever.Server.Character.Game.Who;
using Rebus.Handlers;

namespace NexusForever.Server.Character.Network.Internal.Handler.Who
{
    public class WhoRequestHandler : IHandleMessages<WhoRequestMessage>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly CharacterManager _characterManager;
        private readonly PartitionedRateLimiter<Identity> _rateLimiter;
        private readonly QueryBuilder _queryBuilder;
        private readonly QueryExecutor _queryExecutor;

        public WhoRequestHandler(
            IInternalMessagePublisher messagePublisher,
            CharacterManager characterManager,
            PartitionedRateLimiter<Identity> rateLimiter,
            QueryBuilder queryBuilder,
            QueryExecutor queryExecutor)
        {
            _messagePublisher = messagePublisher;
            _characterManager = characterManager;
            _rateLimiter      = rateLimiter;
            _queryBuilder     = queryBuilder;
            _queryExecutor    = queryExecutor;
        }

        #endregion

        public async Task Handle(WhoRequestMessage message)
        {
            var character = await _characterManager.GetCharacterRemoteAsync(message.Identity.ToQueryIdentity());
            if (character == null)
                return;

            (WhoResult result, List<Game.Character.Character> characters) = await HandleWhoRequest(message, character.CurrentRealmId);
            await _messagePublisher.PublishAsync(new WhoResponseMessage
            {
                Identity   = message.Identity,
                Result     = result,
                Characters = characters.ConvertAll(c => c.ToInternalCharacter())
            });

            return;
        }

        private async Task<(WhoResult, List<Game.Character.Character>)> HandleWhoRequest(WhoRequestMessage message, ushort realmId)
        {
            RateLimitLease lease = await _rateLimiter.AcquireAsync(message.Identity.ToQueryIdentity());
            try
            {
                if (!lease.IsAcquired)
                    return (WhoResult.UnderCooldown, []);

                Query query = _queryBuilder.Build(message);
                query.RealmId = realmId;

                var characters = await _queryExecutor.QueryAsync(query);

                return (WhoResult.OK, characters);
            }
            finally
            {
                lease.Dispose();
            }
        }
    }
}
