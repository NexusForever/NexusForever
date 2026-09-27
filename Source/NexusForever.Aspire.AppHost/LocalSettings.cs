using System.Net;
using System.Net.Sockets;

namespace NexusForever.Aspire.AppHost;

internal sealed class LocalSettings
{
    public string ClientPath { get; set; } = "";
    public string AssetPath { get; set; } = "../../data/assets";
    public string WorldDatabasePath { get; set; } = "../../data/world-database";
    public string RealmHost { get; set; } = "127.0.0.1";
    public string RealmName { get; set; } = "NexusForever";
    public string AccountName { get; set; } = "nexusforever";
    public uint AccountRole { get; set; } = 1;
    public int AuthPort { get; set; } = 23115;
    public int StsPort { get; set; } = 6600;
    public int WorldPort { get; set; } = 24000;
    public bool PhpMyAdmin { get; set; } = true;
    public string MySqlVolume { get; set; } = "nexusforever-aspire-mysql84-data";
    public string RabbitMqVolume { get; set; } = "nexusforever-aspire-rabbitmq-data";

    public void ResolvePaths(string appHostDirectory)
    {
        if (!string.IsNullOrWhiteSpace(ClientPath))
            ClientPath = Path.GetFullPath(ClientPath, appHostDirectory);
        AssetPath = Path.GetFullPath(AssetPath, appHostDirectory);
        WorldDatabasePath = Path.GetFullPath(WorldDatabasePath, appHostDirectory);
    }

    public void Validate(bool checkPorts = true)
    {
        if (!IPAddress.TryParse(RealmHost, out var address) || address.AddressFamily != AddressFamily.InterNetwork
            || address.Equals(IPAddress.Any))
            throw new InvalidOperationException("RealmHost must be the IPv4 address clients use to reach this server.");
        if (string.IsNullOrWhiteSpace(RealmName) || RealmName.Length > 64)
            throw new InvalidOperationException("RealmName must contain 1 to 64 characters.");
        if (string.IsNullOrWhiteSpace(AccountName) || AccountRole is < 1 or > 3)
            throw new InvalidOperationException("Set AccountName and AccountRole (1: Player, 2: GameMaster, 3: Administrator).");

        int[] ports = [AuthPort, StsPort, WorldPort];
        if (ports.Any(p => p is < 1 or > 65535) || ports.Distinct().Count() != ports.Length)
            throw new InvalidOperationException("AuthPort, StsPort, and WorldPort must be distinct ports between 1 and 65535.");

        foreach (int port in checkPorts ? ports : [])
        {
            using var listener = new TcpListener(IPAddress.Any, port);
            try { listener.Start(); }
            catch (SocketException exception)
            {
                throw new InvalidOperationException($"Port {port} is in use. Stop the conflicting service or change the port in appsettings.Local.json.", exception);
            }
        }

        if (!Directory.Exists(WorldDatabasePath) || !Directory.EnumerateFiles(WorldDatabasePath, "*.sql", SearchOption.AllDirectories).Any())
            throw new InvalidOperationException($"No world SQL files in {WorldDatabasePath}. Run ./aspire.sh --setup or set WorldDatabasePath to an existing checkout.");
    }
}
