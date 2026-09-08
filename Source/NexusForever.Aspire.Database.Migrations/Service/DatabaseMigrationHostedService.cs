using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NexusForever.Database.Auth;
using NexusForever.Database.Character;
using NexusForever.Database.Chat;
using NexusForever.Database.Friendship;
using NexusForever.Database.Group;
using NexusForever.Database.Query;
using NexusForever.Database.World;

namespace NexusForever.Aspire.Database.Migrations.Service
{
    public class DatabaseMigrationHostedService : IHostedService
    {
        #region Dependency Injection

        private readonly AuthContext _authContext;
        private readonly CharacterContext _characterContext;
        private readonly WorldContext _worldContext;
        private readonly GroupContext _groupContext;
        private readonly ChatContext _chatContext;
        private readonly FriendshipContext _friendshipContext;
        private readonly QueryContext _queryContext;

        public DatabaseMigrationHostedService(
            AuthContext authContext,
            CharacterContext characterContext,
            WorldContext worldContext,
            GroupContext groupContext,
            ChatContext chatContext,
            FriendshipContext friendshipContext,
            QueryContext queryContext)
        {
            _authContext       = authContext;
            _characterContext  = characterContext;
            _worldContext      = worldContext;
            _groupContext      = groupContext;
            _chatContext       = chatContext;
            _friendshipContext = friendshipContext;
            _queryContext      = queryContext;
        }

        #endregion

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _authContext.Database.MigrateAsync(cancellationToken);
            await _characterContext.Database.MigrateAsync(cancellationToken);
            await _worldContext.Database.MigrateAsync(cancellationToken);
            await _groupContext.Database.MigrateAsync(cancellationToken);
            await _chatContext.Database.MigrateAsync(cancellationToken);
            await _friendshipContext.Database.MigrateAsync(cancellationToken);
            await _queryContext.Database.MigrateAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
