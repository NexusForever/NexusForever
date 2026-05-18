using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Group;
using NexusForever.Database.Group.Model;
using NexusForever.Database.Group.Repository;
using Quartz;

namespace NexusForever.Server.GroupServer.Job
{
    [DisallowConcurrentExecution]
    public class GroupInviteExpiredScheduledJob : IJob
    {
        #region Dependency Injection

        private readonly IServiceProvider _serviceProvider;
        private readonly GroupRepository _repository;
        private readonly GroupContext _context;

        public GroupInviteExpiredScheduledJob(
            IServiceProvider serviceProvider,
            GroupRepository repository,
            GroupContext context)
        {
            _serviceProvider = serviceProvider;
            _repository      = repository;
            _context         = context;
        }

        #endregion

        public async Task Execute(IJobExecutionContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                GroupModel model = await _repository.GetNextGroupWithExpiredInvite();
                if (model == null)
                    break;

                var group = _serviceProvider.GetRequiredService<Group.Group>();
                group.Initialise(model);
                await group.InvitesExpireAsync();

                await _context.SaveChangesAsync();
            }
        }
    }
}
