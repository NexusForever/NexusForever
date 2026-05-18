using System.Text.Json;
using NexusForever.Database.Group;
using NexusForever.Database.Group.Model;
using NexusForever.Database.Group.Repository;
using NexusForever.Network.Internal;
using Quartz;

namespace NexusForever.Server.GroupServer.Job
{
    [DisallowConcurrentExecution]
    public class OutboxScheduledJob : IJob
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly InternalMessageRepository _repository;
        private readonly GroupContext _context;

        public OutboxScheduledJob(
            IInternalMessagePublisher messagePublisher,
            InternalMessageRepository repository,
            GroupContext context)
        {
            _messagePublisher = messagePublisher;
            _repository       = repository;
            _context          = context;
        }

        #endregion

        public async Task Execute(IJobExecutionContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                InternalMessageModel message = await _repository.GetNextMessage();
                if (message == null)
                    break;

                var type = Type.GetType(message.Type);
                if (type == null)
                    continue;

                object payload = JsonSerializer.Deserialize(message.Payload, type);
                await _messagePublisher.PublishAsync(payload);

                message.ProcessedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }
    }
}
