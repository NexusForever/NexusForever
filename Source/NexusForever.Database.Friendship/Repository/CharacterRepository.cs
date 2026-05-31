using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Friendship.Model;

namespace NexusForever.Database.Friendship.Repository
{
    public class CharacterRepository
    {
        private readonly FriendshipContext _context;

        public CharacterRepository(
            FriendshipContext context)
        {
            _context = context;
        }

        public void AddCharacter(CharacterModel model)
        {
            _context.Character.Add(model);
        }

        public async Task<CharacterModel> GetCharacterAsync(ulong id, ushort realmId)
        {
            return await IncludeCharacter(_context.Character)
                .SingleOrDefaultAsync(c => c.CharacterId == id && c.RealmId == realmId);
        }

        public async Task<CharacterModel> GetCharacterAsync(string name, string realmName)
        {
            return await IncludeCharacter(_context.Character)
                .SingleOrDefaultAsync(c => c.Name == name && c.RealmName == realmName);
        }

        private static IQueryable<CharacterModel> IncludeCharacter(IQueryable<CharacterModel> query)
        {
            return query
                .Include(c => c.Account)
                .Include(c => c.Friends)
                .Include(c => c.FriendsInverse)
                .Include(c => c.FriendInvites)
                .Include(c => c.FriendInvitesPending)
                .Include(c => c.Stats);
        }
    }
}
