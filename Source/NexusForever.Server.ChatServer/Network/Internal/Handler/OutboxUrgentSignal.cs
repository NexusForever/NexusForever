using System.Threading.Channels;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler
{
    public class OutboxUrgentSignal
    {
        private readonly Channel<Guid> _channel = Channel.CreateUnboundedPrioritized<Guid>();

        public async Task WriteUrgent(Guid guid)
        {
            await _channel.Writer.WriteAsync(guid);
        }

        public async Task<bool> WaitForUrgent(CancellationToken cancellationToken = default)
        {
            return await _channel.Reader.WaitToReadAsync(cancellationToken);
        }

        public bool ReadUrgent(out Guid guid)
        {
            return _channel.Reader.TryRead(out guid);
        }
    }
}
