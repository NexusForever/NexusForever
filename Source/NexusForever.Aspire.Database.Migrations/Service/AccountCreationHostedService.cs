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
            if (_options.UserName == null || _options.Password == null)
            {
                _log.LogWarning("Account creation options are not configured, skipping account creation.");
                return;
            }

            AccountModel accountModel = _context.Account.SingleOrDefault(a => a.Email == _options.UserName);
            if (accountModel != null)
            {
                _log.LogInformation("Account with username '{UserName}' already exists, skipping account creation.", _options.UserName);
                return;
            }

            (string salt, string vertifier) = PasswordProvider.GenerateSaltAndVerifier(_options.UserName, _options.Password);
            _context.Account.Add(new AccountModel
            {
                Email = _options.UserName,
                S     = salt,
                V     = vertifier
            });

            try
            {
                await _context.SaveChangesAsync();
                _log.LogInformation("Account with username '{UserName}' created successfully.", _options.UserName);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to create account with username '{UserName}'.", _options.UserName);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
