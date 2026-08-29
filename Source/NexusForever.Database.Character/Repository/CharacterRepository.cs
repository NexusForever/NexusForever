using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Character.Model;

namespace NexusForever.Database.Character.Repository
{
    public class CharacterRepository
    {
        #region Dependency Injection

        private readonly CharacterContext context;

        public CharacterRepository(
            CharacterContext context)
        {
            this.context = context;
        }

        #endregion

        public async Task<CharacterModel> GetCharacterAsync(ulong characterId)
        {
            return await context.Character
                .Include(c => c.Stat)
                .SingleOrDefaultAsync(c => c.Id == characterId);
        }

        public async Task<CharacterModel> GetCharacterAsync(string characterName)
        {
            return await context.Character
                .Include(c => c.Stat)
                .SingleOrDefaultAsync(c => c.Name == characterName);
        }
    }
}
