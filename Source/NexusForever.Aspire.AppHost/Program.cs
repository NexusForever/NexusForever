using System.Net;
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

        var authdb      = mysql.AddDatabase("authdb");
        var characterdb = mysql.AddDatabase("characterdb");
        var worlddb     = mysql.AddDatabase("worlddb");
        var groupdb     = mysql.AddDatabase("groupdb");
        var chatdb      = mysql.AddDatabase("chatdb");

        IResourceBuilder<ProjectResource> dbMigration = builder.AddProject<Projects.NexusForever_Aspire_Database_Migrations>("database-migrations")
            .WithReference(authdb)
            .WithReference(characterdb)
            .WithReference(worlddb)
            .WithReference(groupdb)
            .WithReference(chatdb)
            .WaitFor(authdb)
            .WaitFor(characterdb)
            .WaitFor(worlddb)
            .WaitFor(groupdb)
            .WaitFor(chatdb);

        builder.AddProject<Projects.NexusForever_AuthServer>("auth-server")
            .WithNexusForeverTcp(IPAddress.Any, 23115)
            .WithNexusForeverDatabase("Auth", DatabaseProvider.MySql, authdb.Resource)
            .WaitFor(authdb)
            .WaitForCompletion(dbMigration);

        builder.AddProject<Projects.NexusForever_StsServer>("sts-server")
            .WithNexusForeverTcp(IPAddress.Any, 6600)
            .WithNexusForeverDatabase("Auth", DatabaseProvider.MySql, authdb.Resource)
            .WaitFor(authdb)
            .WaitForCompletion(dbMigration);

        IResourceBuilder<ProjectResource> worldServer = builder.AddProject<Projects.NexusForever_WorldServer>("world-server")
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

        IResourceBuilder<ProjectResource> characterApi = builder.AddProject<Projects.NexusForever_API_Character>("character-api")
            .WithNexusForeverHttp(4000)
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
            .WithNexusForeverDatabase("Chat", DatabaseProvider.MySql, chatdb.Resource)
            .WithNexusForeverMessageBroker("ChatServer", BrokerProvider.RabbitMQ, rmq.Resource)
            .WithNexusForeverApi("Character", characterApi.Resource)
            .WaitFor(rmq)
            .WaitFor(chatdb)
            .WaitForCompletion(dbMigration)
            .WaitFor(characterApi);

        DistributedApplication host = builder.Build();
        await host.RunAsync();
    }
}
