using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using NexusForever.Cryptography;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Database.Character;
using NexusForever.Database.Chat;
using NexusForever.Database.Friendship;
using NexusForever.Database.Group;
using NexusForever.Database.Query;
using NexusForever.Database.World;
using NexusForever.Database.World.Model;
using NexusForever.Game.Static.RBAC;
using NexusForever.IO.Map;

static string Required(string name) => Environment.GetEnvironmentVariable(name) is { Length: > 0 } value
    ? value : throw new InvalidOperationException($"Set {name} in .env.");

static MySqlConnectionStringBuilder Connection(string user, string password, string database = "") => new()
{
    Server = "mysql", UserID = user, Password = password, Database = database,
    AllowUserVariables = true, DefaultCommandTimeout = 600
};

static DbContextOptions<T> Options<T>(string database) where T : DbContext =>
    new DbContextOptionsBuilder<T>().UseMySql(
        Connection("nexusforever", Required("MYSQL_PASSWORD"), $"nexus_forever_{database}").ConnectionString,
        new MySqlServerVersion(new Version(8, 4, 0))).Options;

try
{
    if (args is ["check-assets"])
    {
        using (File.OpenRead("/assets/tbl/World.tbl")) { }
        string[] maps = Directory.GetFiles("/assets/map", "*.nfmap");
        if (maps.Length == 0)
            throw new InvalidOperationException("MAP_PATH must contain extracted .nfmap files.");
        foreach (string path in maps)
        {
            using var reader = new BinaryReader(File.OpenRead(path));
            try { new MapFile().ReadHeader(reader); }
            catch (InvalidDataException error)
            {
                throw new InvalidDataException($"{Path.GetFileName(path)}: {error.Message} Set MAP_PATH to assets generated for this checkout.", error);
            }
        }
        Console.WriteLine($"Asset headers valid ({maps.Length} maps). Table files will be loaded by the servers.");
        return 0;
    }

    if (args is ["check-queue", var queue])
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        string credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(
            $"{Required("RABBITMQ_USER")}:{Required("RABBITMQ_PASSWORD")}"));
        client.DefaultRequestHeaders.Authorization = new("Basic", credentials);
        string response = await client.GetStringAsync($"http://rabbitmq:15672/api/queues/%2F/{Uri.EscapeDataString(queue)}");
        return JsonNode.Parse(response)["consumers"].GetValue<int>() > 0 ? 0 : 1;
    }

    if (args is ["configure"])
    {
        string example = Directory.GetFiles(".", "*.example.json").Single();
        string target = example.Replace(".example.json", ".json");
        var config = JsonNode.Parse(await File.ReadAllTextAsync(example));
        foreach (var section in config["Database"].AsObject())
        {
            IEnumerable<JsonNode> connections = section.Value is JsonArray array ? array : new[] { section.Value };
            foreach (var connection in connections)
            {
                connection["Provider"] = "MySql";
                connection["ConnectionString"] = Connection("nexusforever", Required("MYSQL_PASSWORD"),
                    $"nexus_forever_{section.Key.ToLowerInvariant()}").ConnectionString;
            }
        }
        if (config["Network"]?["Internal"] is JsonNode broker)
        {
            broker["Broker"] = "RabbitMQ";
            broker["ConnectionString"] = $"amqp://{Uri.EscapeDataString(Required("RABBITMQ_USER"))}:{Uri.EscapeDataString(Required("RABBITMQ_PASSWORD"))}@rabbitmq:5672/";
        }
        await File.WriteAllTextAsync(target, config.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        return 0;
    }

    // The client protocol requires IPv4.
    string realmHost = Required("REALM_HOST");
    if (!IPAddress.TryParse(realmHost, out var address) || address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork
        || address.Equals(IPAddress.Any))
        throw new InvalidOperationException("REALM_HOST must be the IPv4 address clients use to reach this server (not 0.0.0.0).");
    string realmName = Required("REALM_NAME");
    if (realmName.Length > 64)
        throw new InvalidOperationException("REALM_NAME must be at most 64 characters.");
    ushort realmPort = ushort.Parse(Required("WORLD_PORT"));
    if (realmPort == 0)
        throw new InvalidOperationException("WORLD_PORT must be between 1 and 65535.");
    string email = Required("GAME_ACCOUNT_EMAIL").ToLowerInvariant();
    string password = Required("GAME_ACCOUNT_PASSWORD");
    if (!Enum.TryParse<Role>(Required("GAME_ACCOUNT_ROLE"), out var role)
        || role is not (Role.Player or Role.GameMaster or Role.Administrator))
        throw new InvalidOperationException("GAME_ACCOUNT_ROLE must be Player, GameMaster or Administrator.");

    await using (var root = new MySqlConnection(Connection("root", Required("MYSQL_ROOT_PASSWORD")).ConnectionString))
    {
        await root.OpenAsync();
        await using var createUser = new MySqlCommand("CREATE USER IF NOT EXISTS 'nexusforever'@'%' IDENTIFIED BY @password", root);
        createUser.Parameters.AddWithValue("password", Required("MYSQL_PASSWORD"));
        await createUser.ExecuteNonQueryAsync();
        foreach (string database in new[] { "auth", "character", "world", "chat", "group", "friendship", "query" })
        {
            await using var command = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS `nexus_forever_{database}` CHARACTER SET utf8mb4; GRANT ALL ON `nexus_forever_{database}`.* TO 'nexusforever'@'%'", root);
            await command.ExecuteNonQueryAsync();
        }
    }

    await using var auth = new AuthContext(Options<AuthContext>("auth"));
    await using var character = new CharacterContext(Options<CharacterContext>("character"));
    await using var world = new WorldContext(Options<WorldContext>("world"));
    await using var chat = new ChatContext(Options<ChatContext>("chat"));
    await using var group = new GroupContext(Options<GroupContext>("group"));
    await using var friendship = new FriendshipContext(Options<FriendshipContext>("friendship"));
    await using var query = new QueryContext(Options<QueryContext>("query"));
    foreach (var context in new DbContext[] { auth, character, world, chat, group, friendship, query })
    {
        Console.WriteLine($"Migrating {context.GetType().Name}...");
        await context.Database.MigrateAsync();
    }

    const string dataPath = "/world-database";
    string[] files = Directory.GetFiles(dataPath, "*.sql", SearchOption.AllDirectories);
    if (files.Length == 0)
        throw new InvalidOperationException("No world database SQL files were found.");
    foreach (string file in files.Order(StringComparer.Ordinal))
    {
        string name = Path.GetRelativePath(dataPath, file).Replace('\\', '/');
        byte[] bytes = await File.ReadAllBytesAsync(file);
        string hash = Convert.ToHexStringLower(SHA256.HashData(bytes));
        if (await world.Version.AnyAsync(v => v.FileName == name && v.FileHash == hash))
            continue;

        Console.WriteLine($"Importing {name}...");
        await using var transaction = await world.Database.BeginTransactionAsync();
        // Raw SQL avoids EF treating braces in the dumps as format placeholders.
        await using var command = world.Database.GetDbConnection().CreateCommand();
        command.Transaction = transaction.GetDbTransaction();
        command.CommandTimeout = 600;
        command.CommandText = await File.ReadAllTextAsync(file);
        await command.ExecuteNonQueryAsync();
        // Replace the hash so reverting a dump works too.
        await world.Version.Where(v => v.FileName == name).ExecuteDeleteAsync();
        world.Version.Add(new VersionModel { FileName = name, FileHash = hash, AppliedOn = DateTime.UtcNow });
        await world.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    Console.WriteLine($"World data ready ({files.Length} files).");

    var realm = await auth.Server.SingleAsync(s => s.Id == 1);
    realm.Host = realmHost;
    realm.Name = realmName;
    realm.Port = realmPort;
    if (!await auth.Account.AnyAsync(a => a.Email == email))
    {
        (string salt, string verifier) = PasswordProvider.GenerateSaltAndVerifier(email, password);
        auth.Account.Add(new AccountModel
        {
            Email = email, S = salt, V = verifier,
            AccountRole = [new AccountRoleModel { RoleId = (uint)role }]
        });
        Console.WriteLine($"Creating game account {email}.");
    }
    await auth.SaveChangesAsync();
    Console.WriteLine($"Setup complete. Realm 1: {realmHost}:{realmPort}.");
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine($"Setup failed: {exception.Message}");
    return 1;
}
