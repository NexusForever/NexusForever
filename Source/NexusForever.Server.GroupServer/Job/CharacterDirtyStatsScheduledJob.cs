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
    public class CharacterDirtyStatsScheduledJob : IJob
    {
        #region Dependency Injection

        private readonly IServiceProvider _serviceProvider;
        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly CharacterRepository _repository;
        private readonly GroupContext _context;

        public CharacterDirtyStatsScheduledJob(
            IServiceProvider serviceProvider,
            IInternalMessagePublisher messagePublisher,
            CharacterRepository repository,
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
                CharacterModel model = await _repository.GetNextCharacterWithDirtyStats();
                if (model == null)
                    break;

                var character = _serviceProvider.GetRequiredService<Character.Character>();
                character.Initialise(model);

                foreach (Character.CharacterGroup characterGroup in character.GetGroups())
                {
                    Group.Group group = await characterGroup.GetGroupAsync();
                    if (group == null)
                        continue;

                    GroupMember member = group.GetMember(character.Identity);
                    if (member == null)
                        continue;

                    await _messagePublisher.PublishAsync(new GroupMemberStatsUpdatedMessage
                    {
                        Group  = await group.ToInternalGroup(),
                        Member = await member.ToInternalGroupMember(),
                    });
                }

                character.StatsDirty = false;

                await _context.SaveChangesAsync();
            }
        }
    }
}
