using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NexusForever.Game.Abstract.Loot.Generate;

namespace NexusForever.WorldServer.Service
{
    public class LootHostedService : IHostedService
    {
        #region Dependency Injection

        private readonly ILootGroupManager lootGroupManager;

        public LootHostedService(
            ILootGroupManager lootGroupManager)
        {
            this.lootGroupManager = lootGroupManager;
        }

        #endregion

        public Task StartAsync(CancellationToken cancellationToken)
        {
            lootGroupManager.Initialise();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
