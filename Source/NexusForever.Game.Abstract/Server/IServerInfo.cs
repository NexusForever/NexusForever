using NexusForever.Database.Auth.Model;

namespace NexusForever.Game.Abstract.Server
{
    public interface IServerInfo
    {
        ServerModel Model { get; }
        uint Address { get; }
        bool IsOnline { get; }

        /// <summary>
        /// Attempt to connect to the remote world server asynchronously.
        /// </summary>
        Task PingHostAsync();
    }
}