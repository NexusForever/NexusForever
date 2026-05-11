using NexusForever.Database.Auth.Model;
using NexusForever.Database.Auth.Repository;

namespace NexusForever.API.Account.Account
{
    public class AccountManager
    {
        #region Dependency Injection

        private readonly AccountRepository _repository;

        public AccountManager(
            AccountRepository repository)
        {
            _repository = repository;
        }

        #endregion

        public async Task<Model.Account.Account> GetAccountAsync(uint id)
        {
            AccountModel account = await _repository.GetAccountAsync(id);
            return account?.ToAccount();
        }

        public async Task<Model.Account.Account> GetAccountAsync(string email)
        {
            AccountModel account = await _repository.GetAccountByEmailAsync(email);
            return account?.ToAccount();
        }
    }
}
