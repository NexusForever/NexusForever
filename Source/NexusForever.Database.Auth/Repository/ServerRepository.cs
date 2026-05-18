using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Auth.Model;

namespace NexusForever.Database.Auth.Repository
{
    public class ServerRepository
    {
        #region Dependency Injection

        private readonly AuthContext context;

        public ServerRepository(
            AuthContext context)
        {
            this.context = context;
        }

        #endregion

        public async Task<ServerModel> GetServerAsync(uint serverId)
        {
            return await context.Server.FindAsync((byte)serverId);
        }

        public async Task<ServerModel> GetServerAsync(string serverName)
        {
            return await context.Server.FirstOrDefaultAsync(s => s.Name == serverName);
        }
    }
}
