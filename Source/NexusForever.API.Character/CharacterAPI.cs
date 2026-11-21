using System.Reflection;
using NexusForever.API.Character.Character;
using NexusForever.API.Character.Configuration.Model;
using NexusForever.API.Character.Database;
using NexusForever.API.Character.Endpoint;
using NexusForever.API.Character.Server;
using NexusForever.API.Telemetry;
using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Database.Character;
using NexusForever.Database.Configuration.Model;
using NexusForever.Database.Telemetry;
using NexusForever.Telemetry;
using NLog.Extensions.Logging;
using OpenTelemetry;

namespace NexusForever.API.Character
{
    public static class CharacterAPI
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging
                .ClearProviders()
                .AddConfiguration(builder.Configuration.GetSection("Logging"))
                .AddNLog(new NLogProviderOptions
                {
                    RemoveLoggerFactoryFilter = false
                });

            string basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            builder.Configuration
                .SetBasePath(basePath)
                .AddJsonFile("CharacterAPI.json", false)
                .AddJsonFile("Logging.json", true)
                .AddEnvironmentVariables();

            OpenTelemetryBuilder otb = builder.Services.AddNexusForeverTelemetry(
                builder.Configuration.GetSection("Telemetry"))?
                    .AddAspTracing()
                    .AddEntityFrameworkTracing();

            builder.Services
                .AddAuthDatabase(
                    builder.Configuration.GetSection("Database:Auth")
                        .Get<DatabaseConnectionString>())
                .AddTransient<ContextFactory<CharacterContext>>();

            foreach (DatabaseConnectionStringWithRealm databaseConfiguration in 
                builder.Configuration.GetSection("Database:Character")
                    .Get<List<DatabaseConnectionStringWithRealm>>())
                builder.Services.AddDbContextKeyed<CharacterContext>(databaseConfiguration.RealmId, options => options.UseConfiguration(databaseConfiguration));

            builder.Services
                .AddScoped<ServerManager>()
                .AddScoped<CharacterManager>();

            builder.Host
                .UseWindowsService()
                .UseSystemd();

            WebApplication app = builder.Build();

            app.MapGetCharacterEndpoint();

            app.Run();
        }
    }
}
