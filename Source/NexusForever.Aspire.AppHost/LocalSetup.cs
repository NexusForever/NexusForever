using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NexusForever.Aspire.AppHost;

internal static class LocalSetup
{
    private const string WorldRevision = "79d836576b9da8d7f7321ec3463d3673d496f090";

    public static async Task Configure(string directory, LocalSettings settings, bool hasSavedPassword)
    {
        if (Console.IsInputRedirected)
            throw new InvalidOperationException("Run --setup from a terminal, or supply appsettings.Local.json and the game-password user secret.");

        settings.ResolvePaths(directory);
        settings.ClientPath = Prompt("WildStar installation or Patch directory (optional after asset preparation)", settings.ClientPath);
        settings.AssetPath = Prompt("Asset output directory", settings.AssetPath);
        settings.WorldDatabasePath = Prompt("World database directory (download if missing)", settings.WorldDatabasePath);
        settings.RealmHost = Prompt("Realm IPv4 address", settings.RealmHost);
        settings.RealmName = Prompt("Realm name", settings.RealmName);
        settings.AccountName = Prompt("Initial account username", settings.AccountName);
        settings.AccountRole = (uint)PromptNumber("Initial account role: 1 Player, 2 GameMaster, 3 Administrator", (int)settings.AccountRole, 1, 3);
        settings.AuthPort = PromptNumber("Auth port", settings.AuthPort, 1, 65535);
        settings.StsPort = PromptNumber("STS port", settings.StsPort, 1, 65535);
        settings.WorldPort = PromptNumber("World port", settings.WorldPort, 1, 65535);
        string password;
        do
        {
            Console.Write("Initial account password (leave blank to keep the saved password): ");
            password = ReadPassword();
            if (password.Length == 0 && !hasSavedPassword)
                Console.WriteLine("Enter a password for the initial account.");
        } while (password.Length == 0 && !hasSavedPassword);

        settings.ResolvePaths(directory);
        if (password.Length > 0)
        {
            string secrets = JsonSerializer.Serialize(new Dictionary<string, string> { ["Parameters:game-password"] = password });
            await Run("dotnet", directory, ["user-secrets", "set", "--project", "NexusForever.Aspire.AppHost.csproj"], secrets);
        }

        string path = Path.Combine(directory, "appsettings.Local.json");
        var options = new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };
        var config = File.Exists(path) ? JsonNode.Parse(await File.ReadAllTextAsync(path), documentOptions: options)!.AsObject() : new JsonObject();
        foreach (string key in config.Select(property => property.Key).Where(key =>
            key.Equals("NexusForever", StringComparison.OrdinalIgnoreCase)
            || key.StartsWith("NexusForever:", StringComparison.OrdinalIgnoreCase)).ToArray())
            config.Remove(key);
        config["NexusForever"] = JsonSerializer.SerializeToNode(settings);
        await File.WriteAllTextAsync(path, config.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
        Console.WriteLine($"Saved {path}. Start with ./aspire.sh from the repository root.");
        await DownloadWorldDatabase(settings.WorldDatabasePath);
        settings.Validate(checkPorts: false);
    }

    private static string Prompt(string label, string current)
    {
        Console.Write($"{label} [{current}]: ");
        string value = Console.ReadLine() ?? throw new EndOfStreamException();
        return string.IsNullOrWhiteSpace(value) ? current : value.Trim();
    }

    private static int PromptNumber(string label, int current, int minimum, int maximum)
    {
        while (true)
        {
            if (int.TryParse(Prompt(label, current.ToString()), out int value) && value >= minimum && value <= maximum)
                return value;
            Console.WriteLine($"Enter a number between {minimum} and {maximum}.");
        }
    }

    private static string ReadPassword()
    {
        var result = new StringBuilder();
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
                break;
            if (key.Key == ConsoleKey.Backspace && result.Length > 0)
                result.Length--;
            else if (!char.IsControl(key.KeyChar))
                result.Append(key.KeyChar);
        }
        Console.WriteLine();
        return result.ToString();
    }

    private static async Task DownloadWorldDatabase(string destination)
    {
        if (Directory.Exists(destination))
            return;

        string temporary = destination + ".download-" + Guid.NewGuid().ToString("N");
        Directory.CreateDirectory(temporary);
        try
        {
            Console.WriteLine($"Downloading world database {WorldRevision}...");
            await Run("git", temporary, ["init", "--quiet"]);
            await Run("git", temporary, ["remote", "add", "origin", "https://github.com/NexusForever/NexusForever.WorldDatabase.git"]);
            await Run("git", temporary, ["fetch", "--depth", "1", "origin", WorldRevision]);
            await Run("git", temporary, ["checkout", "--quiet", "--detach", "FETCH_HEAD"]);
            Directory.Move(temporary, destination);
        }
        finally
        {
            if (Directory.Exists(temporary))
                Directory.Delete(temporary, recursive: true);
        }
    }

    private static async Task Run(string command, string directory, string[] arguments, string? input = null)
    {
        var start = new ProcessStartInfo(command)
        {
            WorkingDirectory = directory,
            UseShellExecute = false,
            RedirectStandardInput = input != null
        };
        foreach (string argument in arguments)
            start.ArgumentList.Add(argument);
        using var process = Process.Start(start) ?? throw new InvalidOperationException($"Could not start {command}.");
        if (input != null)
        {
            await process.StandardInput.WriteAsync(input);
            process.StandardInput.Close();
        }
        await process.WaitForExitAsync();
        if (process.ExitCode != 0)
            throw new InvalidOperationException($"{command} exited with code {process.ExitCode}.");
    }
}
