using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using NexusForever.Shared.Database.Auth;
using NexusForever.Shared.Database.Auth.Model;
using NLog;

namespace NexusForever.Shared.Game
{
    public static class ServerManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public class ServerInfo
        {
            public Server Model { get; }
            public uint Address { get; }

            public ServerInfo(Server model)
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
        }

        public static ImmutableList<ServerInfo> Servers { get; private set; }

        public static void Initialise()
        {
            Servers = AuthDatabase.GetServers()
                .Select(s => new ServerInfo(s))
                .ToImmutableList();
        }
    }
}
