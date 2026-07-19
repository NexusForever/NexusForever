using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using NexusForever.Network.Session.Static;
using NexusForever.Shared.Game.Events;
using NLog;

namespace NexusForever.Network.Session
{
    public abstract class NetworkSession : INetworkSession
    {
        protected static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Unique id for <see cref="NetworkSession"/>.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// <see cref="IEvent"/> queue that will be processed during <see cref="NetworkSession"/> update.
        /// </summary>
        public EventQueue Events { get; } = new();

        /// <summary>
        /// Heartbeat to check if <see cref="NetworkSession"/> is still alive.
        /// </summary>
        /// <remarks>
        /// If <see cref="SocketHeartbeat"/> flatlines the <see cref="NetworkSession"/> will be disconnected.
        /// </remarks>
        public SocketHeartbeat Heartbeat { get; } = new();

        private Socket socket;
        private CancellationTokenSource receiveCts;
        private Task receivePipeTask;

        private DisconnectState? disconnectState;

        /// <summary>
        /// Initialise <see cref="NetworkSession"/> with new <see cref="Socket"/> and begin listening for data.
        /// </summary>
        public virtual void OnAccept(Socket newSocket)
        {
            if (socket != null)
                throw new InvalidOperationException();

            Id = Guid.NewGuid().ToString();
            socket = newSocket;

            var stream = new NetworkStream(socket);
            var pipeReader = PipeReader.Create(stream);

            receiveCts = new CancellationTokenSource();
            receivePipeTask = Task.Run(() => RunReceivePipeAsync(pipeReader, receiveCts.Token));

            log.Trace($"New client {Id} connected from {newSocket.RemoteEndPoint}.");
        }

        private async Task RunReceivePipeAsync(PipeReader pipeReader, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    ReadResult result = await pipeReader.ReadAsync(cancellationToken);
                    ReadOnlySequence<byte> buffer = result.Buffer;

                    SequencePosition consumed = OnData(in buffer);
                    pipeReader.AdvanceTo(consumed, buffer.End);

                    if (result.IsCompleted || result.IsCanceled)
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                // normal shutdown via ForceDisconnect or OnDisconnect
            }
            catch (Exception e)
            {
                log.Error(e, $"An exception occured for client {Id} during socket read!");
            }
            finally
            {
                await pipeReader.CompleteAsync();
                ForceDisconnect();
            }
        }

        /// <summary>
        /// Update <see cref="NetworkSession"/> existing id with a new supplied id.
        /// </summary>
        /// <remarks>
        /// This should be used when the default session id can be replaced with a known unique id.
        /// </remarks>
        public void UpdateId(string id)
        {
            log.Trace($"Client {Id} updated id to {id}.");
            Id = id;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public virtual void Update(double lastTick)
        {
            Events.Update(lastTick);

            if (!disconnectState.HasValue)
                Heartbeat.Update(lastTick);

            // Prevents disconnection process happening again
            if (disconnectState == DisconnectState.Complete || disconnectState == DisconnectState.Processing)
                return;

            if (Heartbeat.Flatline || disconnectState == DisconnectState.Pending)
            {
                // no defibrillator is going to save this session
                if (Heartbeat.Flatline)
                    log.Trace($"Client {Id} has flatlined.");

                disconnectState = DisconnectState.Processing;
                OnDisconnect();
            }
        }

        protected virtual void OnDisconnect()
        {
            receiveCts?.Cancel();

            try
            {
                EndPoint remoteEndPoint = socket.RemoteEndPoint;
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

                log.Trace($"Client {Id} disconnected. {remoteEndPoint}");
            }
            catch (Exception e)
            {
                log.Error(e, $"An exception occured for client {Id} during socket close!");
            }

            disconnectState = DisconnectState.Complete;
        }

        /// <summary>
        /// Returns if <see cref="NetworkSession"/> can be disposed.
        /// </summary>
        public virtual bool CanDispose()
        {
            return disconnectState == DisconnectState.Complete && !Events.PendingEvents;
        }

        /// <summary>
        /// Invoked when data is received from the remote client. Returns the consumed position within the buffer.
        /// </summary>
        protected abstract SequencePosition OnData(in ReadOnlySequence<byte> buffer);

        /// <summary>
        /// Send supplied data to remote client on <see cref="Socket"/>.
        /// </summary>
        protected void SendRaw(byte[] data)
        {
            try
            {
                socket.Send(data, 0, data.Length, SocketFlags.None);
            }
            catch (Exception e)
            {
                log.Error(e, $"An exception occured for client {Id} during socket send!");
                ForceDisconnect();
            }
        }

        /// <summary>
        /// Force disconnect of <see cref="NetworkSession"/>.
        /// </summary>
        public void ForceDisconnect()
        {
            if (disconnectState.HasValue)
                return;

            disconnectState = DisconnectState.Pending;
        }
    }
}
