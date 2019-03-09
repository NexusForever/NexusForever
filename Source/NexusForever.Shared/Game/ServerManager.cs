using System.Collections.Immutable;
using System.Linq;
using NexusForever.Shared.Database.Auth;

namespace NexusForever.Shared.Game
{
    public static class ServerManager
    {
        public static ImmutableList<ServerInfo> Servers { get; private set; }
        public static ImmutableList<ServerMessageInfo> ServerMessages { get; private set; }

        public static void Initialise()
        {
            InitialiseServers();
            InitialiseServerMessages();
        }

        private static void InitialiseServers()
        {
            Servers = AuthDatabase.GetServers()
                .Select(s => new ServerInfo(s))
                .ToImmutableList();
        }

        private static void InitialiseServerMessages()
        {
            ServerMessages = AuthDatabase.GetServerMessages()
                .GroupBy(m => m.Index)
                .Select(g => new ServerMessageInfo(g))
                .ToImmutableList();
        }
    }
}
