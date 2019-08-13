using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared.Cryptography;
using NexusForever.Shared.Database.Auth.Model;

namespace NexusForever.Shared.Database.Auth
{
    public static class AuthDatabase
    {
        public static async Task Save(Action<AuthContext> action)
        {
            using (var context = new AuthContext())
            {
                action.Invoke(context);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Selects an <see cref="Account"/> asynchronously that matches the supplied email.
        /// </summary>
        public static async Task<Account> GetAccountAsync(string email)
        {
            using (var context = new AuthContext())
                return await context.Account.SingleOrDefaultAsync(a => a.Email == email);
        }

        /// <summary>
        /// Selects an <see cref="Account"/> asynchronously that matches the supplied email and game token.
        /// </summary>
        public static async Task<Account> GetAccountAsync(string email, Guid guid)
        {
            string gameToken = guid.ToByteArray().ToHexString();
            using (var context = new AuthContext())
                return await context.Account.SingleOrDefaultAsync(a => a.Email == email && a.GameToken == gameToken);
        }

        /// <summary>
        /// Selects an <see cref="Account"/> asynchronously that matches the supplied email and session key.
        /// </summary>
        public static async Task<Account> GetAccountAsync(string email, byte[] sessionKeyBytes)
        {
            string sessionKey = BitConverter.ToString(sessionKeyBytes).Replace("-", "");
            using (var context = new AuthContext())
                return await context.Account
                    .Include(a => a.AccountCostumeUnlock)
                    .Include(a => a.AccountCurrency)
                    .Include(a => a.AccountGenericUnlock)
                    .Include(a => a.AccountKeybinding)
                    .SingleOrDefaultAsync(a => a.Email == email && a.SessionKey == sessionKey);
        }

        /// <summary>
        /// Create a new account with the supplied email and password, the password will have a verifier generated that is inserted into the database.
        /// </summary>
        public static void CreateAccount(string email, string password)
        {
            using (var context = new AuthContext())
            {
                byte[] s = RandomProvider.GetBytes(16u);
                byte[] v = Srp6Provider.GenerateVerifier(s, email, password);

                context.Account.Add(new Account
                {
                    Email = email,
                    S     = s.ToHexString(),
                    V     = v.ToHexString()
                });

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Delete an existing account with the supplied email.
        /// </summary>
        public static bool DeleteAccount(string email)
        {
            // Thanks Rawaho!
            using (var context = new AuthContext())
            {
                Account account = context.Account.SingleOrDefault(a => a.Email == email);
                if (account == null)
                    return false;

                context.Account.Remove(account);
                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Update <see cref="Account"/> with supplied game token asynchronously.
        /// </summary>
        public static async Task UpdateAccountGameToken(Account account, Guid guid)
        {
            account.GameToken = guid.ToByteArray().ToHexString();
            using (var context = new AuthContext())
            {
                EntityEntry<Account> entity = context.Attach(account);
                entity.Property(p => p.GameToken).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Update <see cref="Account"/> with supplied session key asynchronously.
        /// </summary>
        public static async Task UpdateAccountSessionKey(Account account, byte[] sessionKeyBytes)
        {
            account.SessionKey = BitConverter.ToString(sessionKeyBytes).Replace("-", "");
            using (var context = new AuthContext())
            {
                EntityEntry<Account> entity = context.Attach(account);
                entity.Property(p => p.SessionKey).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public static ImmutableList<Server> GetServers()
        {
            using (var context = new AuthContext())
                return context.Server
                    .AsNoTracking()
                    .ToImmutableList();
        }

        public static ImmutableList<ServerMessage> GetServerMessages()
        {
            using (var context = new AuthContext())
                return context.ServerMessage
                    .AsNoTracking()
                    .ToImmutableList();
        }
    }
}
