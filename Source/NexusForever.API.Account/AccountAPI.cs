using System.Reflection;
using NexusForever.API.Account.Account;
using NexusForever.API.Account.Endpoint;
using NexusForever.Database.Auth;
using NexusForever.Database.Configuration.Model;
using NLog.Extensions.Logging;

namespace NexusForever.API.Account
{
    public static class AccountAPI
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Logging
                .ClearProviders()
                .AddNLog();

            string basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            builder.Configuration
                .SetBasePath(basePath)
                .AddJsonFile("AccountAPI.json", false)
                .AddEnvironmentVariables();

            builder.Services
                .AddAuthDatabase(
                    builder.Configuration.GetSection("Database:Auth")
                        .Get<DatabaseConnectionString>());

            builder.Services
                .AddScoped<AccountManager>();

            builder.Host
                .UseWindowsService()
                .UseSystemd();

            WebApplication app = builder.Build();

            app.MapGetAccountEndpoint();

            app.Run();
        }
    }
}
