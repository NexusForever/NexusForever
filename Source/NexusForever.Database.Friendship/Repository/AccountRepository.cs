using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Friendship.Model;

namespace NexusForever.Database.Friendship.Repository
{
    public class AccountRepository
    {
        private readonly FriendshipContext _context;

        public AccountRepository(
            FriendshipContext context)
        {
            _context = context;
        }

        public void AddAccount(AccountModel account)
        {
            _context.Account.Add(account);
        }

        public async Task<AccountModel> GetAccountAsync(uint id)
        {
            return await IncludeAccount(_context.Account)
                .SingleOrDefaultAsync(a => a.AccountId == id);
        }

        public async Task<AccountModel> GetAccountAsync(string email)
        {
            return await _context.Account.FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<AccountModel> GetAccountByNicknameAsync(string nickname)
        {
            return await _context.Account.FirstOrDefaultAsync(a => a.Nickname == nickname);
        }

        private static IQueryable<AccountModel> IncludeAccount(IQueryable<AccountModel> query)
        {
            return query
                .Include(a => a.FriendInvites)
                .Include(a => a.FriendInvitesPending)
                .Include(a => a.Friends)
                .Include(a => a.FriendsInverse);
        }
    }
}
