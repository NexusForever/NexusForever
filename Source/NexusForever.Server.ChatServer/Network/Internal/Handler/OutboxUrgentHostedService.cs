using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NexusForever.Database.Chat;
using NexusForever.Database.Chat.Model;
using NexusForever.Database.Chat.Repository;
using NexusForever.Network.Internal;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler
{
    public class OutboxUrgentHostedService : BackgroundService
    {
        #region Dependency Injection

        private readonly IServiceProvider _serviceProvider;
        private readonly OutboxUrgentSignal _outboxSignal;
        private readonly IInternalMessagePublisher _messagePublisher;

        public OutboxUrgentHostedService(
            IServiceProvider serviceProvider,
            OutboxUrgentSignal outboxSignal,
            IInternalMessagePublisher messagePublisher)
        {
            _serviceProvider  = serviceProvider;
            _outboxSignal     = outboxSignal;
            _messagePublisher = messagePublisher;
        }

        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!await _outboxSignal.WaitForUrgent(stoppingToken))
                    break;

                while (_outboxSignal.ReadUrgent(out Guid guid))
                {
                    IServiceScope scope = _serviceProvider.CreateScope();

                    ChatContext context = scope.ServiceProvider.GetRequiredService<ChatContext>();
                    InternalMessageRepository repository = scope.ServiceProvider.GetRequiredService<InternalMessageRepository>();

                    InternalMessageModel message = await repository.GetMessageAsync(guid);
                    if (message == null)
                        continue;

                    Type type = Type.GetType(message.Type);
                    if (type == null)
                        continue;

                    object payload = JsonSerializer.Deserialize(message.Payload, type);
                    await _messagePublisher.PublishAsync(payload);

                    message.ProcessedAt = DateTime.UtcNow;

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
