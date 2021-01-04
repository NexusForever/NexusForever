using System;
using System.Net.Sockets;

namespace NexusForever.Shared.Network
{
    public abstract class NetworkSession : Session
    {
        /// <summary>
        /// Returns whether the remote client has disconencted, if true <see cref="Socket"/> resources have been released.
        /// </summary>
        public bool Disconnected { get; private set; }

        /// <summary>
        /// Returns whether disconnection of the remote client has been requested.
        /// </summary>
        public bool RequestedDisconnect { get; protected set; }

        public SocketHeartbeat Heartbeat { get; } = new SocketHeartbeat();

        private Socket socket;
        private readonly byte[] buffer = new byte[4096];

        /// <summary>
        /// Initialise <see cref="NetworkSession"/> with new <see cref="Socket"/> and begin listening for data.
        /// </summary>
        public virtual void OnAccept(Socket newSocket)
        {
            if (socket != null)
                throw new InvalidOperationException();

            socket = newSocket;
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveDataCallback, null);

            log.Trace($"New client connected. {newSocket.RemoteEndPoint}");

        }

        public override void Update(double lastTick)
        {
            base.Update(lastTick);

            if (!RequestedDisconnect)
            {
                Heartbeat.Update(lastTick);
                if (Heartbeat.Flatline)
                    RequestedDisconnect = true;
            }
            else if (!Disconnected)
                OnDisconnect();
        }

        protected virtual void OnDisconnect()
        {
            Disconnected = true;
            var remoteEndPoint = socket.RemoteEndPoint;
            socket.Close();

            log.Trace($"Client disconnected. {remoteEndPoint}");
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
                    RequestedDisconnect = true;
                    return;
                }

                byte[] data = new byte[length];
                Buffer.BlockCopy(buffer, 0, data, 0, data.Length);
                OnData(data);

                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveDataCallback, null);
            }
            catch
            {
                RequestedDisconnect = true;
            }
        }

        protected abstract void OnData(byte[] data);

        /// <summary>
        /// Send supplied data to remote client on <see cref="Socket"/>.
        /// </summary>
        protected void SendRaw(byte[] data)
        {
            try
            {
                socket.Send(data, 0, data.Length, SocketFlags.None);
            }
            catch
            {
                RequestedDisconnect = true;
            }
        }
    }
}
