using System.Text.Json;
using NexusForever.Database.Chat;
using NexusForever.Database.Chat.Model;
using NexusForever.Database.Chat.Repository;
using NexusForever.Network.Internal;
using Quartz;

namespace NexusForever.Server.ChatServer.Job
{
    [DisallowConcurrentExecution]
    public class OutboxScheduledJob : IJob
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly InternalMessageRepository _repository;
        private readonly ChatContext _context;

        public OutboxScheduledJob(
            IInternalMessagePublisher messagePublisher,
            InternalMessageRepository repository,
            ChatContext context)
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
                InternalMessageModel message = await _repository.GetNextMessageAsync();
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
