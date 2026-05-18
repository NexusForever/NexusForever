using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Group;
using NexusForever.Database.Group.Model;
using NexusForever.Database.Group.Repository;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Server.GroupServer.Group;
using Quartz;

namespace NexusForever.Server.GroupServer.Job
{
    [DisallowConcurrentExecution]
    public class GroupDirtyPositionScheduledJob : IJob
    {
        #region Dependency Injection

        private readonly IServiceProvider _serviceProvider;
        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly GroupRepository _repository;
        private readonly GroupContext _context;

        public GroupDirtyPositionScheduledJob(
            IServiceProvider serviceProvider,
            IInternalMessagePublisher messagePublisher,
            GroupRepository repository,
            GroupContext context)
        {
            _serviceProvider  = serviceProvider;
            _messagePublisher = messagePublisher;
            _repository       = repository;
            _context          = context;
        }

        #endregion

        public async Task Execute(IJobExecutionContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                GroupModel model = await _repository.GetNextGroupWithDirtyPosition();
                if (model == null)
                    break;

                var group = _serviceProvider.GetRequiredService<Group.Group>();
                group.Initialise(model);

                var memberPositionUpdated = new GroupMemberPositionUpdatedMessage
                {
                    Group          = await group.ToInternalGroup(),
                    UpdatedMembers = []
                };

                foreach (GroupMember groupMember in group.GetMembers())
                {
                    if (!groupMember.PositionDirty)
                        continue;

                    memberPositionUpdated.UpdatedMembers.Add(await groupMember.ToInternalGroupMember());
                    groupMember.PositionDirty = false;
                }

                await _messagePublisher.PublishAsync(memberPositionUpdated);

                await _context.SaveChangesAsync();
            }
        }
    }
}
