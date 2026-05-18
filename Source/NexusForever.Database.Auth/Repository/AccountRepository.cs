using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Auth.Model;

namespace NexusForever.Database.Auth.Repository
{
    public class AccountRepository
    {
        #region Dependency Injection

        private readonly AuthContext context;

        public AccountRepository(
            AuthContext context)
        {
            this.context = context;
        }

        #endregion

        public async Task<AccountModel> GetAccountAsync(uint id)
        {
            return await context.Account.FindAsync(id);
        }

        public async Task<AccountModel> GetAccountByEmailAsync(string email)
        {
            return await context.Account.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
