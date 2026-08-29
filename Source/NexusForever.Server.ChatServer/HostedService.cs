using Microsoft.Extensions.Hosting;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.GameTable.Text.Filter;

namespace NexusForever.Server.ChatServer
{
    public class HostedService : IHostedService
    {
        #region Dependency Injection

        private readonly IGameTableManager _gameTableManager;
        private readonly ITextFilterManager _textFilterManager;

        public HostedService(
            IGameTableManager gameTableManager,
            ITextFilterManager textFilterManager)
        {
            _gameTableManager  = gameTableManager;
            _textFilterManager = textFilterManager;
        }

        #endregion

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var loader = new GameTableLoader()
                .AddGameTable<WordFilterEntry>();

            await _gameTableManager.Initialise(loader);
            _textFilterManager.Initialise();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
