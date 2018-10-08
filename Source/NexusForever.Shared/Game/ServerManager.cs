using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using NexusForever.Shared.Database.Auth;
using NexusForever.Shared.Database.Auth.Model;

namespace NexusForever.Shared.Game
{
    public static class ServerManager
    {
        public class ServerInfo
        {
            public Server Model { get; }
            public uint Address { get; }

            public ServerInfo(Server model)
            {
                IPAddress ipAddress = IPAddress.Parse(model.Host);
                Address = (uint)IPAddress.HostToNetworkOrder(BitConverter.ToInt32(ipAddress.GetAddressBytes()));
                Model   = model;
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
