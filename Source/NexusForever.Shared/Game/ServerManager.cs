using System.Collections.Immutable;
using System.Linq;
using NexusForever.Shared.Database.Auth;

namespace NexusForever.Shared.Game
{
    public sealed class ServerManager : Singleton<ServerManager>
    {
        public ImmutableList<ServerInfo> Servers { get; private set; }
        public ImmutableList<ServerMessageInfo> ServerMessages { get; private set; }

        private ServerManager()
        {
        }

        public void Initialise()
        {
            InitialiseServers();
            InitialiseServerMessages();
        }

        private void InitialiseServers()
        {
            Servers = AuthDatabase.GetServers()
                .Select(s => new ServerInfo(s))
                .ToImmutableList();
        }

        private void InitialiseServerMessages()
        {
            ServerMessages = AuthDatabase.GetServerMessages()
                .GroupBy(m => m.Index)
                .Select(g => new ServerMessageInfo(g))
                .ToImmutableList();
        }
    }
}
