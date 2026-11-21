using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Query.Model;

namespace NexusForever.Database.Query.Repository
{
    public class CharacterRepository
    {
        #region Dependency Injection

        private readonly QueryContext _context;

        public CharacterRepository(
            QueryContext context)
        {
            _context = context;
        }

        #endregion

        public void AddCharacter(CharacterModel character)
        {
            _context.Character.Add(character);
        }

        public async Task<CharacterModel> GetCharacterAsync(ulong characterId, uint realmId)
        {
            return await _context.Character.SingleOrDefaultAsync(c => c.CharacterId == characterId && c.RealmId == realmId);
        }
    }
}
