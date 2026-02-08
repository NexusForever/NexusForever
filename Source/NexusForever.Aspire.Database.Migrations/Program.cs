using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NexusForever.Aspire.Database.Migrations.Configuration.Model;
using NexusForever.Aspire.Database.Migrations.Service;
using NexusForever.Database.Auth;
using NexusForever.Database.Character;
using NexusForever.Database.Chat;
using NexusForever.Database.Group;
using NexusForever.Database.World;
using NLog.Extensions.Logging;

namespace NexusForever.Aspire.Database.Migrations
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var builder = new HostBuilder()
                .ConfigureAppConfiguration(cb =>
                {
                    cb.SetBasePath(basePath)
                        .AddJsonFile("AspireMigrations.json", false)
                        .AddEnvironmentVariables();
                })
                .ConfigureLogging(l =>
                {
                    l.ClearProviders();
                    l.AddNLog();
                })
                .ConfigureServices((hb, sc) =>
                {
                    sc.AddOptions<AccountCreationOptions>()
                        .Bind(hb.Configuration.GetSection("AccountCreation"));

                    sc.AddOptions<WorldDatabaseOptions>()
                        .Bind(hb.Configuration.GetSection("WorldDatabase"));

                    sc.AddHostedService<DatabaseMigrationHostedService>();
                    sc.AddHostedService<AccountCreationHostedService>();
                    sc.AddHostedService<WorldDatabaseHostedService>();
                    sc.AddHostedService<FinishHostedService>();

                    sc.AddScoped(sp =>
                    {
                        var options = sp.GetService<DbContextOptions<AuthContext>>();
                        return new AuthContext(options);
                    });
                    sc.AddScoped(sp =>
                    {
                        var options = sp.GetService<DbContextOptions<CharacterContext>>();
                        return new CharacterContext(options);
                    });
                    sc.AddScoped(sp =>
                    {
                        var options = sp.GetService<DbContextOptions<WorldContext>>();
                        return new WorldContext(options);
                    });

                    sc.AddDbContext<AuthContext>(options =>
                    {
                        var connectionString = hb.Configuration.GetConnectionString("authdb");
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    });
                    sc.AddDbContext<CharacterContext>(options =>
                    {
                        var connectionString = hb.Configuration.GetConnectionString("characterdb");
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    });
                    sc.AddDbContext<WorldContext>(options =>
                    {
                        var connectionString = hb.Configuration.GetConnectionString("worlddb");
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    });
                    sc.AddDbContext<GroupContext>(options =>
                    {
                        var connectionString = hb.Configuration.GetConnectionString("groupdb");
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    });
                    sc.AddDbContext<ChatContext>(options =>
                    {
                        var connectionString = hb.Configuration.GetConnectionString("chatdb");
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    });
                });

            IHost host = builder.Build();
            await host.RunAsync();
        }
    }
}
