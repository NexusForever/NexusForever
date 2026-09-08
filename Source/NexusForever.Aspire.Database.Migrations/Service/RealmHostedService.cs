using System.Net;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NexusForever.Aspire.Database.Migrations.Configuration.Model;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;

namespace NexusForever.Aspire.Database.Migrations.Service
{
    public class RealmHostedService(AuthContext context, IOptions<RealmOptions> options) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            RealmOptions realm = options.Value;
            if (!IPAddress.TryParse(realm.Host, out var address) || address.AddressFamily != AddressFamily.InterNetwork
                || address.Equals(IPAddress.Any) || realm.Port == 0)
                throw new InvalidOperationException("Set a reachable realm IPv4 address and a port between 1 and 65535.");
            if (string.IsNullOrWhiteSpace(realm.Name) || realm.Name.Length > 64)
                throw new InvalidOperationException("Realm name must contain 1 to 64 characters.");

            var server = await context.Server.SingleOrDefaultAsync(s => s.Id == 1, cancellationToken);
            if (server == null)
            {
                server = new ServerModel { Id = 1 };
                context.Server.Add(server);
            }
            server.Host = realm.Host;
            server.Port = realm.Port;
            server.Name = realm.Name;
            await context.SaveChangesAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
