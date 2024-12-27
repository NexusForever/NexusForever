using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Auth.Model;
using NexusForever.Database.Configuration.Model;
using NLog;

namespace NexusForever.Database.Auth
{
    [Database(DatabaseType.Auth)]
    public class AuthDatabase : IDatabase
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private IConnectionString config;

        public void Initialise(IConnectionString connectionString)
        {
            config = connectionString;
        }

        public async Task Save(Action<AuthContext> action)
        {
            using var context = new AuthContext(config);
            action.Invoke(context);
            await context.SaveChangesAsync();
        }

        public void Migrate()
        {
            using var context = new AuthContext(config);

            List<string> migrations = context.Database.GetPendingMigrations().ToList();
            if (migrations.Count > 0)
            {
                log.Info($"Applying {migrations.Count} authentication database migration(s)...");
                foreach (string migration in migrations)
                    log.Info(migration);

                context.Database.Migrate();
            }
        }

        /// <summary>
        /// Selects an <see cref="AccountModel"/> asynchronously that matches the supplied email.
        /// </summary>
        public async Task<AccountModel> GetAccountByEmailAsync(string email)
        {
            using var context = new AuthContext(config);
            return await context.Account.SingleOrDefaultAsync(a => a.Email == email);
        }

        /// <summary>
        /// Selects an <see cref="AccountModel"/> asynchronously that matches the supplied email and game token.
        /// </summary>
        public async Task<AccountModel> GetAccountByGameTokenAsync(string email, string gameToken)
        {
            using var context = new AuthContext(config);
            return await context.Account
                .Include(a => a.AccountSuspension)
                .SingleOrDefaultAsync(a => a.Email == email && a.GameToken == gameToken);
        }

        /// <summary>
        /// Selects an <see cref="AccountModel"/> asynchronously that matches the supplied email and session key.
        /// </summary>
        public async Task<AccountModel> GetAccountBySessionKeyAsync(string email, string sessionKey)
        {
            using var context = new AuthContext(config);
            return await context.Account
                .AsSplitQuery()
                .Include(a => a.AccountCostumeUnlock)
                .Include(a => a.AccountCurrency)
                .Include(a => a.AccountGenericUnlock)
                .Include(a => a.AccountKeybinding)
                .Include(a => a.AccountEntitlement)
                .Include(a => a.AccountPermission)
                .Include(a => a.AccountRole)
                .SingleOrDefaultAsync(a => a.Email == email && a.SessionKey == sessionKey);
        }

        /// <summary>
        /// Selects an <see cref="AccountModel"/> asynchronously that matches the supplied external reference type and value.
        /// </summary>
        public async Task<List<AccountModel>> GetAccountsByExternalReference(string referenceType, string referenceValue)
        {
            using var context = new AuthContext(config);
            return await context.Account
                .Include(a => a.AccountExternalReference)
                .Where(a => a.AccountExternalReference.Any(r => r.Type == referenceType && r.Value == referenceValue))
                .ToListAsync();
        }

        /// <summary>
        /// Returns if an account with the given username already exists.
        /// </summary>
        public bool AccountExists(string email)
        {
            using var context = new AuthContext(config);
            return context.Account.SingleOrDefault(a => a.Email == email) != null;
        }

        /// <summary>
        /// Create a new account with the supplied email, salt and password verifier that is inserted into the database.
        /// </summary>
        public void CreateAccount(string email, string s, string v, uint role)
        {
            if (AccountExists(email))
                throw new InvalidOperationException($"Account with that username already exists.");

            CreateAccountAsync(email, s, v, role, []).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Create a new account with the supplied email, salt and password verifier and external references that is inserted into the database.
        /// </summary>
        public async Task CreateAccountAsync(string email, string s, string v, uint role, IEnumerable<(string referenceType, string referenceValue)> externalReferences)
        {
            using var context = new AuthContext(config);

            var model = new AccountModel
            {
                Email = email,
                S     = s,
                V     = v
            };

            model.AccountRole.Add(new AccountRoleModel
            {
                RoleId = role
            });

            foreach ((string referenceType, string referenceValue) in externalReferences)
            {
                model.AccountExternalReference.Add(new AccountExternalReferenceModel
                {
                    Type  = referenceType,
                    Value = referenceValue,
                });
            }

            context.Account.Add(model);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete an existing account with the supplied email.
        /// </summary>
        public bool DeleteAccount(string email)
        {
            using var context = new AuthContext(config);
            AccountModel account = context.Account.SingleOrDefault(a => a.Email == email);
            if (account == null)
                return false;

            context.Account.Remove(account);
            return context.SaveChanges() > 0;
        }

        /// <summary>
        /// Update <see cref="AccountModel"/> with supplied game token asynchronously.
        /// </summary>
        public async Task UpdateAccountGameToken(uint accountId, string gameToken)
        {
            using var context = new AuthContext(config);
            EntityEntry<AccountModel> entity = context.Attach(new AccountModel
            {
                Id        = accountId,
                GameToken = gameToken
            });
            entity.Property(p => p.GameToken).IsModified = true;
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Update <see cref="AccountModel"/> with supplied session key asynchronously.
        /// </summary>
        public async Task UpdateAccountSessionKey(uint accountId, string sessionKey)
        {
            await using var context = new AuthContext(config);
            EntityEntry<AccountModel> entity = context.Attach(new AccountModel
            {
                Id         = accountId,
                SessionKey = sessionKey
            });
            entity.Property(p => p.SessionKey).IsModified = true;
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Update <see cref="AccountModel"/> with supplied salt and verifier asynchronously.
        /// </summary>
        public async Task UpdateAccountPasswordSaltAndVerifier(uint accountId, string s, string v)
        {
            await using var context = new AuthContext(config);
            EntityEntry<AccountModel> entity = context.Attach(new AccountModel
            {
                Id = accountId,
                S  = s,
                V  = v
            });

            entity.Property(p => p.S).IsModified = true;
            entity.Property(p => p.V).IsModified = true;

            await context.SaveChangesAsync();
        }

        public ImmutableList<ServerModel> GetServers()
        {
            using var context = new AuthContext(config);
            return context.Server
                .AsNoTracking()
                .ToImmutableList();
        }

        public ImmutableList<ServerMessageModel> GetServerMessages()
        {
            using var context = new AuthContext(config);
            return context.ServerMessage
                .AsNoTracking()
                .ToImmutableList();
        }

        public ImmutableList<PermissionModel> GetPermissions()
        {
            using var context = new AuthContext(config);
            return context.Permission
                .AsNoTracking()
                .ToImmutableList();
        }

        public ImmutableList<RoleModel> GetRoles()
        {
            using var context = new AuthContext(config);
            return context.Role
                .Include(r => r.RolePermission)
                .AsNoTracking()
                .ToImmutableList();
        }

        public void BanAccount(uint accountId, string reason, DateTime? endTime)
        {
            using var context = new AuthContext(config);
            context.AccountSuspension.Add(new AccountSuspensionModel
            {
                Id      = accountId,
                Reason  = reason,
                EndTime = endTime,
            });

            context.SaveChanges();
        }
    }
}
