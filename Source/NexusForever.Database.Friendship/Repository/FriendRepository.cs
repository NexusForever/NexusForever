using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Friendship.Model;

namespace NexusForever.Database.Friendship.Repository
{
    public class FriendRepository
    {
        private readonly FriendshipContext _context;

        public FriendRepository(
            FriendshipContext context)
        {
            _context = context;
        }

        public void AddFriendInvite(FriendInviteModel invite)
        {
            _context.FriendInvite.Add(invite);
        }

        public void RemoveFriendInvite(FriendInviteModel invite)
        {
            _context.FriendInvite.Remove(invite);
        }

        public async Task<FriendInviteModel> GetFriendInviteAsync(ulong id)
        {
            return await _context.FriendInvite.FindAsync(id);
        }

        public void AddFriend(FriendModel friend)
        {
            _context.Friend.Add(friend);
        }

        public void RemoveFriend(FriendModel friend)
        {
            _context.Friend.Remove(friend);
        }

        public async Task<FriendModel> GetFriendAsync(ulong id)
        {
            return await _context.Friend.FindAsync(id);
        }

        public async Task<FriendModel> GetFriendAsync(ulong characterId, ushort realmId, ulong friendCharacterId, ushort friendRealmId)
        {
            return await _context.Friend
                .FirstOrDefaultAsync(f => f.InviterCharacterId == characterId && f.InviterRealmId == realmId
                    && f.InviteeCharacterId == friendCharacterId && f.InviteeRealmId == friendRealmId);
        }
    }
}
