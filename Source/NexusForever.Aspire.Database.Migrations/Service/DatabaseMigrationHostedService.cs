using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NexusForever.Database.Auth;
using NexusForever.Database.Character;
using NexusForever.Database.Chat;
using NexusForever.Database.Group;
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

        public DatabaseMigrationHostedService(
            AuthContext authContext,
            CharacterContext characterContext,
            WorldContext worldContext,
            GroupContext groupContext,
            ChatContext chatContext)
        {
            _authContext      = authContext;
            _characterContext = characterContext;
            _worldContext     = worldContext;
            _groupContext     = groupContext;
            _chatContext      = chatContext;
        }

        #endregion

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _authContext.Database.MigrateAsync();
            await _characterContext.Database.MigrateAsync();
            await _worldContext.Database.MigrateAsync();
            await _groupContext.Database.MigrateAsync();
            await _chatContext.Database.MigrateAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
