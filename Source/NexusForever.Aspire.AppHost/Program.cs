using System.Net;
using Aspire.Hosting.Dotnet;
using NexusForever.Aspire.AppHost;
using NexusForever.Database;
using NexusForever.Network.Internal.Static;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        //builder.AddDockerComposeEnvironment("nexus-forever");

        var rmq = builder.AddRabbitMQ("rmq")
            .WithManagementPlugin();

        var mysql = builder.AddMySql("mysql")
            .WithPhpMyAdmin()
            .WithDataVolume("mysql-data");

        var authdb       = mysql.AddDatabase("authdb");
        var characterdb  = mysql.AddDatabase("characterdb");
        var worlddb      = mysql.AddDatabase("worlddb");
        var groupdb      = mysql.AddDatabase("groupdb");
        var chatdb       = mysql.AddDatabase("chatdb");
        var friendshipdb = mysql.AddDatabase("friendshipdb");
        var querydb      = mysql.AddDatabase("querydb");

        IResourceBuilder<DotnetProjectResource> dbMigration = builder.AddDotnetProject("database-migrations", "..\\NexusForever.Aspire.Database.Migrations")
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

        builder.AddDotnetProject("auth-server", "..\\NexusForever.AuthServer")
            .WithNexusForeverTcp(IPAddress.Any, 23115)
            .WithNexusForeverDatabase("Auth", DatabaseProvider.MySql, authdb.Resource)
            .WaitFor(authdb)
            .WaitForCompletion(dbMigration);

        builder.AddDotnetProject("sts-server", "..\\NexusForever.StsServer")
            .WithNexusForeverTcp(IPAddress.Any, 6600)
            .WithNexusForeverDatabase("Auth", DatabaseProvider.MySql, authdb.Resource)
            .WaitFor(authdb)
            .WaitForCompletion(dbMigration);

        IResourceBuilder<DotnetProjectResource> worldServer = builder.AddDotnetProject("world-server", "..\\NexusForever.WorldServer")
            .WithNexusForeverTcp(IPAddress.Any, 24000)
            .WithNexusForeverHttp(5000)
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

        IResourceBuilder<DotnetProjectResource> accountApi = builder.AddDotnetProject("account-api", "..\\NexusForever.API.Account")
            .WithNexusForeverHttp(4001)
            .WithNexusForeverDatabase("Auth", DatabaseProvider.MySql, authdb.Resource)
            .WaitFor(authdb)
            .WaitForCompletion(dbMigration);

        IResourceBuilder<DotnetProjectResource> characterApi = builder.AddDotnetProject("character-api", "..\\NexusForever.API.Character")
            .WithNexusForeverHttp(4000)
            .WithNexusForeverDatabase("Auth", DatabaseProvider.MySql, authdb.Resource)
            .WithNexusForeverDatabase("Character:0", DatabaseProvider.MySql, characterdb.Resource)
            .WithEnvironment("Database:Character:0:RealmId", "1")
            .WaitFor(authdb)
            .WaitFor(characterdb)
            .WaitForCompletion(dbMigration);

        builder.AddDotnetProject("group-server", "..\\NexusForever.Server.GroupServer")
            .WithNexusForeverDatabase("Group", DatabaseProvider.MySql, groupdb.Resource)
            .WithNexusForeverMessageBroker("GroupServer", BrokerProvider.RabbitMQ, rmq.Resource)
            .WithNexusForeverApi("Character", characterApi.Resource)
            .WaitFor(rmq)
            .WaitFor(groupdb)
            .WaitForCompletion(dbMigration)
            .WaitFor(characterApi);

        builder.AddDotnetProject("chat-server", "..\\NexusForever.Server.ChatServer")
            .WithNexusForeverDatabase("Chat", DatabaseProvider.MySql, chatdb.Resource)
            .WithNexusForeverMessageBroker("ChatServer", BrokerProvider.RabbitMQ, rmq.Resource)
            .WithNexusForeverApi("Character", characterApi.Resource)
            .WaitFor(rmq)
            .WaitFor(chatdb)
            .WaitForCompletion(dbMigration)
            .WaitFor(characterApi);

        builder.AddDotnetProject("friendship-server", "..\\NexusForever.Server.Friendship")
            .WithNexusForeverDatabase("Friendship", DatabaseProvider.MySql, friendshipdb.Resource)
            .WithNexusForeverMessageBroker("FriendshipServer", BrokerProvider.RabbitMQ, rmq.Resource)
            .WithNexusForeverApi("Account", accountApi.Resource)
            .WithNexusForeverApi("Character", characterApi.Resource)
            .WaitFor(rmq)
            .WaitFor(friendshipdb)
            .WaitForCompletion(dbMigration)
            .WaitFor(accountApi)
            .WaitFor(characterApi);

        builder.AddDotnetProject("character-server", "..\\NexusForever.Server.Character")
            .WithNexusForeverDatabase("Query", DatabaseProvider.MySql, querydb.Resource)
            .WithNexusForeverMessageBroker("CharacterServer", BrokerProvider.RabbitMQ, rmq.Resource)
            .WithNexusForeverApi("Character", characterApi.Resource)
            .WaitFor(rmq)
            .WaitFor(querydb)
            .WaitFor(characterApi);

        DistributedApplication host = builder.Build();
        await host.RunAsync();
    }
}
