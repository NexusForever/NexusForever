using System.Net;
using Microsoft.Extensions.Configuration;
using NexusForever.Aspire.AppHost;
using NexusForever.Database;
using NexusForever.Network.Internal.Static;

internal class Program
{
    private static async Task Main(string[] args)
    {
        bool setup = args.Contains("--setup");
        string[] appArgs = args.Where(a => a != "--setup").ToArray();
        var builder = DistributedApplication.CreateBuilder(appArgs);
        builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true)
            .AddUserSecrets<Program>(optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(appArgs);
        var settings = builder.Configuration.GetSection("NexusForever").Get<LocalSettings>() ?? new();
        if (setup)
        {
            await LocalSetup.Configure(builder.AppHostDirectory, settings,
                !string.IsNullOrEmpty(builder.Configuration["Parameters:game-password"]));
            return;
        }
        settings.ResolvePaths(builder.AppHostDirectory);
        settings.Validate();

        var assets = builder.AddProject<Projects.NexusForever_MapGenerator>("assets")
            .WithArgs("--prepare", "--output", settings.AssetPath);
        if (!string.IsNullOrWhiteSpace(settings.ClientPath))
            assets.WithArgs("--patchPath", settings.ClientPath);
        var gamePassword = builder.AddParameter("game-password", secret: true);

        var rmq = builder.AddRabbitMQ("rmq")
            .WithManagementPlugin()
            .WithDataVolume(settings.RabbitMqVolume)
            .WithEnvironment("RABBITMQ_NODENAME", "rabbit@nexusforever")
            .WithContainerRuntimeArgs("--hostname", "nexusforever");

        var mysql = builder.AddMySql("mysql")
            .WithImageTag("8.4")
            .WithDataVolume(settings.MySqlVolume);
        if (settings.PhpMyAdmin)
            mysql.WithPhpMyAdmin();

        var authdb       = mysql.AddDatabase("authdb");
        var characterdb  = mysql.AddDatabase("characterdb");
        var worlddb      = mysql.AddDatabase("worlddb");
        var groupdb      = mysql.AddDatabase("groupdb");
        var chatdb       = mysql.AddDatabase("chatdb");
        var friendshipdb = mysql.AddDatabase("friendshipdb");
        var querydb      = mysql.AddDatabase("querydb");

        IResourceBuilder<ProjectResource> dbMigration = builder.AddProject<Projects.NexusForever_Aspire_Database_Migrations>("database-migrations")
            .WithEnvironment("WorldDatabase:Path", settings.WorldDatabasePath)
            .WithEnvironment("AccountCreation:Username", settings.AccountName)
            .WithEnvironment("AccountCreation:Password", gamePassword)
            .WithEnvironment("AccountCreation:RoleId", settings.AccountRole.ToString())
            .WithEnvironment("Realm:Host", settings.RealmHost)
            .WithEnvironment("Realm:Name", settings.RealmName)
            .WithEnvironment("Realm:Port", settings.WorldPort.ToString())
            .WaitForCompletion(assets)
            .WithReference(authdb)
            .WithReference(characterdb)
            .WithReference(worlddb)
            .WithReference(groupdb)
            .WithReference(chatdb)
            .WithReference(friendshipdb)
            .WithReference(querydb)
            .WaitFor(authdb)
            .WaitFor(characterdb)
            .WaitFor(worlddb)
            .WaitFor(groupdb)
            .WaitFor(chatdb)
            .WaitFor(friendshipdb)
            .WaitFor(querydb);

        builder.AddProject<Projects.NexusForever_AuthServer>("auth-server")
            .WithNexusForeverTcp(IPAddress.Any, settings.AuthPort)
            .WithNexusForeverDatabase("Auth", DatabaseProvider.MySql, authdb.Resource)
            .WaitFor(authdb)
            .WaitForCompletion(dbMigration);

        builder.AddProject<Projects.NexusForever_StsServer>("sts-server")
            .WithNexusForeverTcp(IPAddress.Any, settings.StsPort)
            .WithNexusForeverDatabase("Auth", DatabaseProvider.MySql, authdb.Resource)
            .WaitFor(authdb)
            .WaitForCompletion(dbMigration);

        IResourceBuilder<ProjectResource> worldServer = builder.AddProject<Projects.NexusForever_WorldServer>("world-server")
            .WithNexusForeverTcp(IPAddress.Any, settings.WorldPort)
            .WithNexusForeverHttp()
            .WithHttpHealthCheck("/console.html")
            .WithEnvironment("GameTable:GameTablePath", Path.Combine(settings.AssetPath, "tbl"))
            .WithEnvironment("Realm:Map:MapPath", Path.Combine(settings.AssetPath, "map"))
            .WithNexusForeverDatabase("Auth", DatabaseProvider.MySql, authdb.Resource)
            .WithNexusForeverDatabase("Character", DatabaseProvider.MySql, characterdb.Resource)
            .WithNexusForeverDatabase("World", DatabaseProvider.MySql, worlddb.Resource)
            .WithNexusForeverMessageBroker("WorldServer_1", BrokerProvider.RabbitMQ, rmq.Resource)
            .WithEnvironment("Realm:RealmId", "1")
            .WaitFor(authdb)
            .WaitFor(characterdb)
            .WaitFor(worlddb)
            .WaitFor(rmq)
            .WaitForCompletion(dbMigration);

        worldServer.WithEnvironment(c =>
        {
            if (c.Resource.TryGetUrls(out var urls))
            {
                foreach (ResourceUrlAnnotation url in urls)
                {
                    if (url.Endpoint?.Scheme != "http")
                        continue;

                    url.DisplayText = "Web Console";
                    url.Url = new UriBuilder(url.Url) { Path = "console.html" }.ToString();
                }
            }
        });

        IResourceBuilder<ProjectResource> accountApi = builder.AddProject<Projects.NexusForever_API_Account>("account-api")
            .WithNexusForeverHttp()
            .WithHttpHealthCheck("/health")
            .WithNexusForeverDatabase("Auth", DatabaseProvider.MySql, authdb.Resource)
            .WaitFor(authdb)
            .WaitForCompletion(dbMigration);

        IResourceBuilder<ProjectResource> characterApi = builder.AddProject<Projects.NexusForever_API_Character>("character-api")
            .WithNexusForeverHttp()
            .WithHttpHealthCheck("/health")
            .WithNexusForeverDatabase("Auth", DatabaseProvider.MySql, authdb.Resource)
            .WithNexusForeverDatabase("Character:0", DatabaseProvider.MySql, characterdb.Resource)
            .WithEnvironment("Database:Character:0:RealmId", "1")
            .WaitFor(authdb)
            .WaitFor(characterdb)
            .WaitForCompletion(dbMigration);

        builder.AddProject<Projects.NexusForever_Server_GroupServer>("group-server")
            .WithNexusForeverDatabase("Group", DatabaseProvider.MySql, groupdb.Resource)
            .WithNexusForeverMessageBroker("GroupServer", BrokerProvider.RabbitMQ, rmq.Resource)
            .WithNexusForeverApi("Character", characterApi.Resource)
            .WaitFor(rmq)
            .WaitFor(groupdb)
            .WaitForCompletion(dbMigration)
            .WaitFor(characterApi);

        builder.AddProject<Projects.NexusForever_Server_ChatServer>("chat-server")
            .WithEnvironment("GameTable:GameTablePath", Path.Combine(settings.AssetPath, "tbl"))
            .WithNexusForeverDatabase("Chat", DatabaseProvider.MySql, chatdb.Resource)
            .WithNexusForeverMessageBroker("ChatServer", BrokerProvider.RabbitMQ, rmq.Resource)
            .WithNexusForeverApi("Character", characterApi.Resource)
            .WaitFor(rmq)
            .WaitFor(chatdb)
            .WaitForCompletion(dbMigration)
            .WaitFor(characterApi);

        builder.AddProject<Projects.NexusForever_Server_Friendship>("friendship-server")
            .WithEnvironment("GameTable:GameTablePath", Path.Combine(settings.AssetPath, "tbl"))
            .WithNexusForeverDatabase("Friendship", DatabaseProvider.MySql, friendshipdb.Resource)
            .WithNexusForeverMessageBroker("FriendshipServer", BrokerProvider.RabbitMQ, rmq.Resource)
            .WithNexusForeverApi("Account", accountApi.Resource)
            .WithNexusForeverApi("Character", characterApi.Resource)
            .WaitFor(rmq)
            .WaitFor(friendshipdb)
            .WaitForCompletion(dbMigration)
            .WaitFor(accountApi)
            .WaitFor(characterApi);

        builder.AddProject<Projects.NexusForever_Server_Character>("character-server")
            .WithNexusForeverDatabase("Query", DatabaseProvider.MySql, querydb.Resource)
            .WithNexusForeverMessageBroker("CharacterServer", BrokerProvider.RabbitMQ, rmq.Resource)
            .WithNexusForeverApi("Character", characterApi.Resource)
            .WaitFor(rmq)
            .WaitFor(querydb)
            .WaitForCompletion(dbMigration)
            .WaitFor(characterApi);

        foreach (var project in builder.Resources.OfType<ProjectResource>())
            builder.CreateResourceBuilder(project).WithEnvironment("NEXUSFOREVER_ASPIRE", "1");

        DistributedApplication host = builder.Build();
        await host.RunAsync();
    }
}
