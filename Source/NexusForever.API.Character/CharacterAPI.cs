using System.Reflection;
using NexusForever.API.Character.Character;
using NexusForever.API.Character.Configuration.Model;
using NexusForever.API.Character.Database;
using NexusForever.API.Character.Endpoint;
using NexusForever.API.Character.Server;
using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Database.Character;
using NexusForever.Database.Configuration.Model;
using NLog.Extensions.Logging;

namespace NexusForever.API.Character
{
    public static class CharacterAPI
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging
                .ClearProviders()
                .AddNLog();

            string basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            builder.Configuration
                .SetBasePath(basePath)
                .AddJsonFile("CharacterAPI.json", false)
                .AddEnvironmentVariables();

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
