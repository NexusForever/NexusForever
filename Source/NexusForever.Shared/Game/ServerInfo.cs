using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NexusForever.Database.Auth.Model;
using NLog;

namespace NexusForever.Shared.Game
{
    public class ServerInfo
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public ServerModel Model { get; }
        public uint Address { get; }
        public bool IsOnline { get; private set; }

        public ServerInfo(ServerModel model)
        {
            try
            {
                if (!IPAddress.TryParse(model.Host, out IPAddress ipAddress))
                {
                    // find first IPv4 address, client doesn't support IPv6 as address is sent as 4 bytes
                    ipAddress = Dns.GetHostEntry(model.Host)
                        .AddressList
                        .First(a => a.AddressFamily == AddressFamily.InterNetwork);
                }

                Address = (uint)IPAddress.HostToNetworkOrder(BitConverter.ToInt32(ipAddress.GetAddressBytes()));
                Model   = model;
            }
            catch (Exception e)
            {
                log.Fatal(e, $"Failed to load server entry id: {model.Id}, host: {model.Host} from the database!");
                throw;
            }
        }

        /// <summary>
        /// Attempt to connect to the remote world server asynchronously.
        /// </summary>
        public async Task PingHostAsync()
        {
            using var client = new TcpClient();
            await client.ConnectAsync(Model.Host, Model.Port).ContinueWith(task =>
            {
                IsOnline = !task.IsFaulted;
                return task;
            });
        }
    }
}
