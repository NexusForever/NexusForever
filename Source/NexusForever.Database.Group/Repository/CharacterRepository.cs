using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Group.Model;

namespace NexusForever.Database.Group.Repository
{
    public class CharacterRepository
    {
        #region Dependency Injection

        private readonly GroupContext _context;

        public CharacterRepository(
            GroupContext context)
        {
            _context = context;
        }

        #endregion

        public void AddCharacter(CharacterModel character)
        {
            _context.Character.Add(character);
        }

        public void RemoveCharacter(CharacterModel character)
        {
            _context.Character.Remove(character);
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

        public async Task<CharacterModel> GetNextCharacterWithDirtyStats()
        {
            return await 
                IncludeCharacter(_context.Character)
                .Where(c => c.StatsDirty)
                .FirstOrDefaultAsync();
        }

        public async Task<CharacterModel> GetNextCharacterWithDirtyRealm()
        {
            return await 
                IncludeCharacter(_context.Character)
                .Where(c => c.RealmDirty)
                .FirstOrDefaultAsync();
        }

        private IQueryable<CharacterModel> IncludeCharacter(IQueryable<CharacterModel> query)
        {
            return query
                .AsSingleQuery()
                .Include(c => c.Invite)
                .Include(c => c.Groups)
                .Include(c => c.Stats)
                .Include(c => c.Properties);
        }
    }
}
