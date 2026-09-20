using System.Net;
using System.Net.Sockets;
using System.Reflection;
using NexusForever.Network.Session;
using Xunit;

namespace NexusForever.Network.Tests
{
    public class NetworkSessionTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task DisconnectClosesSocketAndCompletesSession(bool disconnectSocketFirst)
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();

            using var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await client.ConnectAsync(listener.LocalEndpoint, timeout.Token);
            using Socket server = await listener.AcceptSocketAsync(timeout.Token);

            var session = new TestSession();
            session.OnAccept(server);

            if (disconnectSocketFirst)
            {
                // A disconnected socket still owns a handle that must be closed.
                server.Disconnect(false);
                Assert.False(server.SafeHandle.IsClosed);
            }

            session.ForceDisconnect();
            session.Update(0);

            Assert.True(server.SafeHandle.IsClosed);
            Assert.True(session.CanDispose());
            Assert.Equal(1, session.DisconnectCount);

            // Subsequent ticks and disconnect requests must not repeat cleanup.
            session.ForceDisconnect();
            session.Update(0);
            Assert.Equal(1, session.DisconnectCount);
        }

        [Fact]
        public void DisconnectClosesSocketWhenShutdownThrows()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var session = new TestSession();

            // Inject an unconnected socket to exercise the Shutdown exception path without
            // starting a receive callback, which can change the socket's connection state.
            FieldInfo socketField = typeof(NetworkSession).GetField("socket", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(socketField);
            socketField.SetValue(session, socket);

            session.ForceDisconnect();
            session.Update(0);

            Assert.True(socket.SafeHandle.IsClosed);
            Assert.True(session.CanDispose());
            Assert.Equal(1, session.DisconnectCount);
        }

        private sealed class TestSession : NetworkSession
        {
            public int DisconnectCount { get; private set; }

            protected override uint OnData(byte[] data) => 0;

            protected override void OnDisconnect()
            {
                DisconnectCount++;
                base.OnDisconnect();
            }
        }
    }
}
