using System.Text.Json;
using NexusForever.Database.Chat.Model;
using NexusForever.Database.Chat.Repository;
using NexusForever.Network.Internal;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler
{
    public class OutboxMessagePublisher : IInternalMessagePublisher
    {
        private readonly List<Guid> _urgentMessages = [];

        #region Dependency Injection

        private readonly InternalMessageRepository _repository;
        private readonly OutboxUrgentSignal _outboxSignal;

        public OutboxMessagePublisher(
            InternalMessageRepository repository,
            OutboxUrgentSignal outboxSignal)
        {
            _repository = repository;
            _outboxSignal = outboxSignal;
        }

        #endregion

        /// <summary>
        /// Publish a message to the internal message broker via an outbox table.
        /// </summary>
        /// <param name="message">Message to publish.</param>
        public async Task PublishAsync(object message)
        {
            InternalMessageModel internalMessage = await CreateMessage(message);
            _repository.AddMessage(internalMessage);
        }

        /// <summary>
        /// Publish a message to the internal message broker via an outbox table.
        /// </summary>
        /// <remarks>
        /// The message will be marked as urgent and will be processed immediately.
        /// If the immediate processing fails it will be handled like a normal outbox message.
        /// </remarks>
        /// <param name="message">Message to publish.</param>
        public async Task PublishUrgentAsync(object message)
        {
            InternalMessageModel internalMessage = await CreateMessage(message);
            _repository.AddMessage(internalMessage);
            _urgentMessages.Add(internalMessage.Id);
        }

        /// <summary>
        /// Send all urgent messages for immediate processing.
        /// </summary>
        public async Task FlushUrgentMessages()
        {
            foreach (Guid messageGuid in _urgentMessages)
                await _outboxSignal.WriteUrgent(messageGuid);

            _urgentMessages.Clear();
        }

        private async Task<InternalMessageModel> CreateMessage(object message)
        {
            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, message, message.GetType());

            stream.Position = 0;
            using var reader = new StreamReader(stream);
            string payload = await reader.ReadToEndAsync();

            var model = new InternalMessageModel
            {
                Id        = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Type      = message.GetType().AssemblyQualifiedName,
                Payload   = payload,
            };

            return model;
        }
    }
}
