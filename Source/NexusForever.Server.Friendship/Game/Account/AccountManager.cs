using Microsoft.Extensions.DependencyInjection;
using NexusForever.API.Account.Client;
using NexusForever.Database.Friendship.Model;
using NexusForever.Database.Friendship.Repository;

namespace NexusForever.Server.Friendship.Game.Account
{
    public class AccountManager
    {
        // scoped account cache to prevent multiple database calls for the same account during a request
        private readonly Dictionary<uint, Account> _accountCache = [];
        private readonly Dictionary<string, Account> _accountEmailCache = [];
        private readonly Dictionary<string, Account> _accountNicknameCache = [];

        #region Dependency Injection

        private readonly AccountRepository _repository;
        private readonly AccountAPIClient _apiClient;
        private readonly IServiceProvider _serviceProvider;

        public AccountManager(
            AccountRepository repository,
            AccountAPIClient apiClient,
            IServiceProvider serviceProvider)
        {
            _repository      = repository;
            _apiClient       = apiClient;
            _serviceProvider = serviceProvider;
        }

        #endregion

        /// <summary>
        /// Get an <see cref="Account"/> with the specified account id.
        /// </summary>
        /// <remarks>
        /// If the account does not exist in the database it will not be fetched from the API.
        /// A local cache is used to prevent multiple database calls for the same account during a single request.
        /// </remarks>
        /// <param name="accountId">Id of the <see cref="Account"/> to return.</param>
        public async Task<Account> GetAccountAsync(uint accountId)
        {
            if (_accountCache.TryGetValue(accountId, out Account account))
                return account;

            AccountModel databaseAccount = await _repository.GetAccountAsync(accountId);
            if (databaseAccount == null)
                return null;
            
            return InitialiseAccount(databaseAccount);
        }

        /// <summary>
        /// Get an <see cref="Account"/> with the specified account id.
        /// </summary>
        /// <remarks>
        /// If the account does not exist in the database it will be fetched from the API and saved to the local database.
        /// A local cache is used to prevent multiple database calls for the same account during a single request.
        /// </remarks>
        /// <param name="accountId">Id of the <see cref="Account"/> to return.</param>
        public async Task<Account> GetAccountRemoteAsync(uint accountId)
        {
            if (_accountCache.TryGetValue(accountId, out Account account))
                return account;

            AccountModel databaseAccount = await _repository.GetAccountAsync(accountId);
            if (databaseAccount == null)
            {
                API.Model.Account.Account apiAccount = await _apiClient.GetAccountAsync(accountId);
                if (apiAccount == null)
                    return null;

                databaseAccount = apiAccount.ToDatabaseAccount();
                _repository.AddAccount(databaseAccount);
            }

            return InitialiseAccount(databaseAccount);
        }

        /// <summary>
        /// Get an <see cref="Account"/> with the specified email address.
        /// </summary>
        /// <remarks>
        /// If the account does not exist in the database it will not be fetched from the API.
        /// A local cache is used to prevent multiple database calls for the same account during a single request.
        /// </remarks>
        /// <param name="email">Email of the <see cref="Account"/> to return.</param>
        public async Task<Account> GetAccountByEmailAsync(string email)
        {
            if (_accountEmailCache.TryGetValue(email, out Account account))
                return account;

            // TODO: should this also fetch from the API?
            AccountModel databaseAccount = await _repository.GetAccountAsync(email);
            if (databaseAccount == null)
                return null;

            return InitialiseAccount(databaseAccount);
        }

        /// <summary>
        /// Get an <see cref="Account"/> with the specified nickname (public display name).
        /// </summary>
        /// <remarks>
        /// If the account does not exist in the database it will not be fetched from the API.
        /// A local cache is used to prevent multiple database calls for the same account during a single request.
        /// </remarks>
        /// <param name="nickname">Nickname of the <see cref="Account"/> to return.</param>
        public async Task<Account> GetAccountByNickname(string nickname)
        {
            if (_accountNicknameCache.TryGetValue(nickname, out Account account))
                return account;

            AccountModel databaseAccount = await _repository.GetAccountByNicknameAsync(nickname);
            if (databaseAccount == null)
                return null;

            return InitialiseAccount(databaseAccount);
        }

        private Account InitialiseAccount(AccountModel model)
        {
            Account account = _serviceProvider.GetRequiredService<Account>();
            account.Initialise(model);

            _accountCache.Add(account.Id, account);
            _accountEmailCache.Add(account.Email, account);

            if (account.Nickname != null)
                _accountNicknameCache.Add(account.Nickname, account);

            return account;
        }
    }
}
