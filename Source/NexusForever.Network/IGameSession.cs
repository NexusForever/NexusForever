using NexusForever.Network.Message;

namespace NexusForever.Network
{
    public interface IGameSession : INetworkSession
    {
        /// <summary>
        /// Determines if queued incoming packets can be processed during a world update.
        /// </summary>
        bool CanProcessIncomingPackets { get; set; }

        /// <summary>
        /// Determines if queued outgoing packets can be processed during a world update.
        /// </summary>
        bool CanProcessOutgoingPackets { get; set; }

        /// <summary>
        /// Enqueue <see cref="IWritable"/> to be sent to the client.
        /// </summary>
        void EnqueueMessage(IWritable message);

        /// <summary>
        /// Enqueue <see cref="IWritable"/> to be sent encrypted to the client.
        /// </summary>
        void EnqueueMessageEncrypted(IWritable message);

        void EnqueueMessageEncrypted(uint opcode, string hex);

        /// <summary>
        /// Flush all queued packets to the client.
        /// </summary>
        public void FlushPackets();
    }
}