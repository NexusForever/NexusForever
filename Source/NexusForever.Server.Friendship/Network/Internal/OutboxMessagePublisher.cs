using System.Text.Json;
using NexusForever.Database.Friendship.Model;
using NexusForever.Database.Friendship.Repository;
using NexusForever.Network.Internal;

namespace NexusForever.Server.Friendship.Network.Internal
{
    // TODO: Rawaho, standardise this across all servers and move to a shared library.
    public class OutboxMessagePublisher : IInternalMessagePublisher
    {
        #region Dependency Injection

        private readonly InternalMessageRepository _repository;

        public OutboxMessagePublisher(
            InternalMessageRepository repository)
        {
            _repository = repository;
        }

        #endregion

        /// <summary>
        /// Publish a message to the internal message broker via an outbox table.
        /// </summary>
        /// <param name="message"></param>
        public async Task PublishAsync(object message)
        {
            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, message, message.GetType());

            stream.Position = 0;
            using var reader = new StreamReader(stream);
            string payload = await reader.ReadToEndAsync();

            _repository.AddMessage(new InternalMessageModel
            {
                Id        = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Type      = message.GetType().AssemblyQualifiedName,
                Payload   = payload,
            });
        }
    }
}
