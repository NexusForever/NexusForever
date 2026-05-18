using NexusForever.Database.Auth.Model;
using NexusForever.Database.Auth.Repository;

namespace NexusForever.API.Character.Server
{
    public class ServerManager
    {
        #region Dependency Injection

        private readonly ServerRepository _serverRepository;

        public ServerManager(
            ServerRepository serverRepository)
        {
            _serverRepository = serverRepository;
        }

        #endregion

        /// <summary>
        /// Get a realm from the database.
        /// </summary>
        /// <param name="realmId">Realm id of the server to return.</param>
        public async Task<Server> GetServerAsync(uint realmId)
        {
            ServerModel server = await _serverRepository.GetServerAsync(realmId);
            return server?.ToServer();
        }

        /// <summary>
        /// Get a realm from the database.
        /// </summary>
        /// <param name="realmName">Realm name of the server to return.</param>
        public async Task<Server> GetServerAsync(string realmName)
        {
            ServerModel server = await _serverRepository.GetServerAsync(realmName);
            return server?.ToServer();
        }
    }
}
