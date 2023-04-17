using System.Net;
using System.Net.Sockets;
using NexusForever.Network.Static;
using NexusForever.Shared.Game.Events;
using NLog;

namespace NexusForever.Network
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
        private readonly byte[] buffer = new byte[4096];
        private int bufferOffset;

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
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveDataCallback, null);

            log.Trace($"New client {Id} connected from {newSocket.RemoteEndPoint}.");
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

            if (Heartbeat.Flatline || disconnectState == DisconnectState.Pending)
            {
                // no defibrillator is going to save this session
                if (Heartbeat.Flatline)
                    log.Trace($"Client {Id} has flatlined.");

                OnDisconnect();
            }
        }

        protected virtual void OnDisconnect()
        {
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
        /// Invoked with <see cref="IAsyncResult"/> when data from the <see cref="Socket"/> is received.
        /// </summary>
        private void ReceiveDataCallback(IAsyncResult ar)
        {
            try
            {
                int length = socket.EndReceive(ar);
                if (length == 0)
                {
                    ForceDisconnect();
                    return;
                }

                byte[] data = new byte[length + bufferOffset];
                Buffer.BlockCopy(buffer, 0, data, 0, data.Length);
                bufferOffset = (int)OnData(data);

                // if we have data that wasn't processed move it to the start of the buffer
                // any new data will be amended to it
                if (bufferOffset != 0)
                    Buffer.BlockCopy(buffer, data.Length - bufferOffset, buffer, 0, bufferOffset);

                socket.BeginReceive(buffer, bufferOffset, buffer.Length - bufferOffset, SocketFlags.None, ReceiveDataCallback, null);
            }
            catch (Exception e)
            {
                log.Error(e, $"An exception occured for client {Id} during socket read!");
                ForceDisconnect();
            }
        }

        protected abstract uint OnData(byte[] data);

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
        /// Forece disconnect of <see cref="NetworkSession"/>.
        /// </summary>
        public void ForceDisconnect()
        {
            if (disconnectState.HasValue)
                return;

            disconnectState = DisconnectState.Pending;
        }
    }
}
