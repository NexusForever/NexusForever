using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NexusForever.Aspire.Database.Migrations.Configuration.Model;
using NexusForever.Cryptography;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;

namespace NexusForever.Aspire.Database.Migrations.Service
{
    public class AccountCreationHostedService : IHostedService
    {
        #region Dependency Injection

        private readonly ILogger<AccountCreationHostedService> _log;
        private readonly AccountCreationOptions _options;
        private readonly AuthContext _context;

        public AccountCreationHostedService(
            ILogger<AccountCreationHostedService> log,
            IOptions<AccountCreationOptions> options,
            AuthContext context)
        {
            _log     = log;
            _options = options.Value;
            _context = context;
        }

        #endregion

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_options.UserName) || string.IsNullOrEmpty(_options.Password))
                throw new InvalidOperationException("Set the initial account username and password.");
            if (_options.RoleId is < 1 or > 3)
                throw new InvalidOperationException("Account role must be 1 (Player), 2 (GameMaster), or 3 (Administrator).");

            string userName = _options.UserName.ToLowerInvariant();
            AccountModel accountModel = await _context.Account.SingleOrDefaultAsync(a => a.Email == userName, cancellationToken);
            if (accountModel != null)
            {
                _log.LogInformation("Account with username '{UserName}' already exists, skipping account creation.", _options.UserName);
                return;
            }

            (string salt, string verifier) = PasswordProvider.GenerateSaltAndVerifier(userName, _options.Password);
            _context.Account.Add(new AccountModel
            {
                Email = userName,
                S     = salt,
                V     = verifier,
                AccountRole = [new AccountRoleModel { RoleId = _options.RoleId }]
            });

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                _log.LogInformation("Account with username '{UserName}' created successfully.", _options.UserName);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to create account with username '{UserName}'.", _options.UserName);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
