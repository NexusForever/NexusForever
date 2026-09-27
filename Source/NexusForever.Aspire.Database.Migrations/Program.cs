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
using NexusForever.Database.Friendship;
using NexusForever.Database.Group;
using NexusForever.Database.Query;
using NexusForever.Database.World;
using NexusForever.Shared.Configuration;
using NLog.Extensions.Logging;
using MySqlConnector;

namespace NexusForever.Aspire.Database.Migrations
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            string basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var builder = new HostBuilder()
                .ConfigureAppConfiguration(cb =>
                {
                    cb.SetBasePath(basePath)
                        .AddNexusForeverJson("AspireMigrations.json")
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

                    sc.AddOptions<RealmOptions>()
                        .Bind(hb.Configuration.GetSection("Realm"));

                    string Connection(string name) => new MySqlConnectionStringBuilder(hb.Configuration.GetConnectionString(name))
                    {
                        AllowUserVariables = true
                    }.ConnectionString;

                    sc.AddHostedService<DatabaseMigrationHostedService>();
                    sc.AddHostedService<WorldDatabaseHostedService>();
                    sc.AddHostedService<AccountCreationHostedService>();
                    sc.AddHostedService<RealmHostedService>();
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
                        var connectionString = Connection("authdb");
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    });
                    sc.AddDbContext<CharacterContext>(options =>
                    {
                        var connectionString = Connection("characterdb");
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    });
                    sc.AddDbContext<WorldContext>(options =>
                    {
                        var connectionString = Connection("worlddb");
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    });
                    sc.AddDbContext<GroupContext>(options =>
                    {
                        var connectionString = Connection("groupdb");
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    });
                    sc.AddDbContext<ChatContext>(options =>
                    {
                        var connectionString = Connection("chatdb");
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    });
                    sc.AddDbContext<FriendshipContext>(options =>
                    {
                        var connectionString = Connection("friendshipdb");
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    });
                    sc.AddDbContext<QueryContext>(options =>
                    {
                        var connectionString = Connection("querydb");
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    });
                });

            try
            {
                using IHost host = builder.Build();
                await host.RunAsync();
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"Database setup failed: {exception}");
                return 1;
            }
        }
    }
}
