using NexusForever.Database.Friendship.Model;

namespace NexusForever.Database.Friendship.Repository
{
    public class AccountFriendRepository
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;

        public AccountFriendRepository(
            FriendshipContext context)
        {
            _context = context;
        }

        #endregion

        public async Task<FriendAccountInviteModel> GetFriendInviteAsync(ulong id)
        {
            return await _context.FriendAccountInvite.FindAsync(id);
        }

        public void AddFriendInvite(FriendAccountInviteModel model)
        {
            _context.FriendAccountInvite.Add(model);
        }

        public void RemoveFriendInvite(FriendAccountInviteModel model)
        {
            _context.FriendAccountInvite.Remove(model);
        }

        public async Task<FriendAccountModel> GetFriendAsync(ulong id)
        {
            return await _context.FriendAccount.FindAsync(id);
        }

        public void AddFriend(FriendAccountModel model)
        {
            _context.FriendAccount.Add(model);
        }

        public void RemoveFriend(FriendAccountModel model)
        {
            _context.FriendAccount.Remove(model);
        }
    }
}
