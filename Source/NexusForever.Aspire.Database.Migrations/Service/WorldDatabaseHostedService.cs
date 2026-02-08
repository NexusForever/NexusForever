using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NexusForever.Aspire.Database.Migrations.Configuration.Model;
using NexusForever.Database.World;
using NexusForever.Database.World.Model;

namespace NexusForever.Aspire.Database.Migrations.Service
{
    public class WorldDatabaseHostedService : IHostedService
    {
        #region Dependency Injection

        private readonly ILogger<WorldDatabaseHostedService> _log;
        private readonly WorldDatabaseOptions _options;
        private readonly WorldContext _context;

        public WorldDatabaseHostedService(
            ILogger<WorldDatabaseHostedService> log,
            IOptions<WorldDatabaseOptions> options,
            WorldContext context)
        {
            _log     = log;
            _options = options.Value;
            _context = context;
        }

        #endregion

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.Path == null)
            {
                _log.LogWarning("World database options are not configured. Skipping migration.");
                return;
            }

            if (!Directory.Exists(_options.Path))
            {
                _log.LogWarning("World database migrations path does not exist. Skipping migration.");
                return;
            }

            foreach (string filePath in Directory.GetFiles(_options.Path, "*.sql", SearchOption.AllDirectories))
            {
                string fileName    = Path.GetFileName(filePath);
                string fileContent = File.ReadAllText(filePath);
                string fileHash    = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(fileContent)));

                if (_context.Version.Any(v => v.FileName == fileName && v.FileHash == fileHash))
                {
                    _log.LogInformation("Skipping already applied world database migration: {FileName}", fileName);
                    continue;
                }

                fileContent = Regex.Replace(fileContent, @"/\*.*?\*/", "", RegexOptions.Singleline);
                fileContent = Regex.Replace(fileContent, @"--.*?$", "", RegexOptions.Multiline);
                fileContent = fileContent.Trim();

                _log.LogInformation("Applying world database migration: {FileName}", fileName);
                try
                {
                    await _context.Database.ExecuteSqlRawAsync(fileContent);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Failed to apply world database migration: {FileName}", fileName);
                    continue;
                }
                _log.LogInformation("Applied world database migration: {FileName}", fileName);

                _context.Version.Add(new VersionModel
                {
                    FileName = fileName,
                    FileHash = fileHash,
                    AppliedOn = DateTime.UtcNow
                });

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
