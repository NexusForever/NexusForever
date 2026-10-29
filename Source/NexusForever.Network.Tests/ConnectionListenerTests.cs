using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging.Abstractions;
using NexusForever.Network.Session;
using NexusForever.Shared;
using Xunit;

namespace NexusForever.Network.Tests
{
    public class ConnectionListenerTests
    {
        [Fact]
        public async Task StartReportsOccupiedPortAndCanBeRetried()
        {
            using var occupied = new TcpListener(IPAddress.Loopback, 0);
            occupied.Server.ExclusiveAddressUse = true;
            occupied.Start();
            uint port = (uint)((IPEndPoint)occupied.LocalEndpoint).Port;
            var listener = CreateListener(port);

            Exception error = await Task.Run(() => Record.Exception(listener.Start))
                .WaitAsync(TimeSpan.FromSeconds(3));
            Assert.IsType<SocketException>(error);

            occupied.Stop();
            listener.Start();
            listener.Shutdown();
        }

        [Fact]
        public async Task ShutdownWaitsForSessionCallbackToFinish()
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            using var releaseCallback = new ManualResetEventSlim();
            var callbackEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var shutdownEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            uint port = GetAvailablePort();
            var listener = CreateListener(port);
            listener.OnNewSession += _ =>
            {
                callbackEntered.SetResult();
                releaseCallback.Wait(timeout.Token);
            };
            listener.Start();
            Task shutdown = null;
            try
            {
                // Let the idle accept loop yield before blocking a session callback.
                await Task.Delay(100, timeout.Token);
                using var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await client.ConnectAsync(IPAddress.Loopback, (int)port, timeout.Token);
                await callbackEntered.Task.WaitAsync(timeout.Token);
                shutdown = Task.Run(() =>
                {
                    shutdownEntered.SetResult();
                    listener.Shutdown();
                });
                await shutdownEntered.Task.WaitAsync(timeout.Token);

                Task completed = await Task.WhenAny(shutdown, Task.Delay(200, timeout.Token));
                Assert.NotSame(shutdown, completed);
            }
            finally
            {
                releaseCallback.Set();
                if (shutdown != null)
                    await shutdown.WaitAsync(timeout.Token);
                else
                    listener.Shutdown();
            }
        }

        [Fact]
        public async Task ListenerCanRestartAndAcceptConnections()
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            uint port = GetAvailablePort();
            var listener = CreateListener(port);
            for (int i = 0; i < 3; i++)
            {
                var accepted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                NewSessionEvent<TestSession> onNewSession = _ => accepted.SetResult();
                listener.OnNewSession += onNewSession;
                listener.Start();
                try
                {
                    using var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    await client.ConnectAsync(IPAddress.Loopback, (int)port, timeout.Token);
                    await accepted.Task.WaitAsync(timeout.Token);
                }
                finally
                {
                    listener.Shutdown();
                    listener.OnNewSession -= onNewSession;
                }
            }
        }

        private static uint GetAvailablePort()
        {
            using var reservation = new TcpListener(IPAddress.Loopback, 0);
            reservation.Start();
            return (uint)((IPEndPoint)reservation.LocalEndpoint).Port;
        }

        private static ConnectionListener<TestSession> CreateListener(uint port)
        {
            var listener = new ConnectionListener<TestSession>(
                NullLogger<ConnectionListener<TestSession>>.Instance, new TestSessionFactory());
            listener.Initialise(IPAddress.Loopback, port);
            return listener;
        }

        private sealed class TestSessionFactory : IFactory<TestSession>
        {
            public TestSession Resolve() => new();
        }

        private sealed class TestSession : NetworkSession
        {
            public override void OnAccept(Socket socket) => socket.Dispose();
            protected override uint OnData(byte[] data) => 0;
        }
    }
}
