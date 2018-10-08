using System;
using System.Net.Sockets;

namespace NexusForever.Shared.Network
{
    public abstract class NetworkSession : Session
    {
        public bool Disconnected { get; private set; }
        public SocketHeartbeat Heartbeat { get; } = new SocketHeartbeat();

        private Socket socket;
        private readonly byte[] buffer = new byte[4096];

        public virtual void OnAccept(Socket newSocket)
        {
            socket = newSocket;
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveDataCallback, null);

            log.Trace("New client connected.");
        }

        public override void Update(double lastTick)
        {
            base.Update(lastTick);

            Heartbeat.Update(lastTick);
            if (Heartbeat.Flatline)
                OnDisconnect();
        }

        private void ReceiveDataCallback(IAsyncResult ar)
        {
            try
            {
                int length = socket.EndReceive(ar);
                if (length == 0)
                {
                    OnDisconnect();
                    return;
                }

                byte[] data = new byte[length];
                Buffer.BlockCopy(buffer, 0, data, 0, data.Length);
                OnData(data);

                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveDataCallback, null);
            }
            catch
            {
                OnDisconnect();
            }
        }

        protected void SendRaw(byte[] data)
        {
            try
            {
                socket.Send(data, 0, data.Length, SocketFlags.None);
            }
            catch
            {
                OnDisconnect();
            }
        }
        
        protected abstract void OnData(byte[] data);

        protected virtual void OnDisconnect()
        {
            Disconnected = true;
            socket.Close();

            log.Trace("Client disconnected.");
        }
    }
}
