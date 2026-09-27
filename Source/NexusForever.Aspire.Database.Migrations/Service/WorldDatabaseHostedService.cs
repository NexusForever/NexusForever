using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
            if (string.IsNullOrWhiteSpace(_options.Path) || !Directory.Exists(_options.Path))
                throw new DirectoryNotFoundException($"World database directory does not exist: {_options.Path}");

            string[] files = Directory.GetFiles(_options.Path, "*.sql", SearchOption.AllDirectories)
                .Order(StringComparer.Ordinal).ToArray();
            if (files.Length == 0)
                throw new InvalidOperationException($"No world SQL files found in {_options.Path}.");

            var nameCounts = files.GroupBy(Path.GetFileName).ToDictionary(g => g.Key, g => g.Count());
            foreach (string filePath in files)
            {
                string fileName    = Path.GetRelativePath(_options.Path, filePath).Replace('\\', '/');
                string legacyName  = nameCounts[Path.GetFileName(filePath)] == 1 ? Path.GetFileName(filePath) : fileName;
                string fileContent = await File.ReadAllTextAsync(filePath, cancellationToken);
                string fileHash    = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(fileContent)));

                var versions = _context.Version.Where(v => v.FileName == fileName || v.FileName == legacyName);
                var latest = await versions.OrderByDescending(v => v.AppliedOn).FirstOrDefaultAsync(cancellationToken);
                if (latest?.FileHash == fileHash)
                {
                    _log.LogInformation("Skipping already applied world database migration: {FileName}", fileName);
                    continue;
                }

                _log.LogInformation("Applying world database migration: {FileName}", fileName);
                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    await using var command = _context.Database.GetDbConnection().CreateCommand();
                    command.Transaction = transaction.GetDbTransaction();
                    command.CommandTimeout = 600;
                    command.CommandText = fileContent;
                    await command.ExecuteNonQueryAsync(cancellationToken);

                    await versions.ExecuteDeleteAsync(cancellationToken);
                    _context.Version.Add(new VersionModel
                    {
                        FileName = fileName,
                        FileHash = fileHash,
                        AppliedOn = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Failed to apply world database migration: {FileName}", fileName);
                    throw;
                }
                _log.LogInformation("Applied world database migration: {FileName}", fileName);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
