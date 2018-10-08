using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NexusForever.Shared.Database.Character.Model;

namespace NexusForever.Shared.Database.Character
{
    public static class CharacterDatabase
    {
        public static ulong GetNextCharacterId()
        {
            using (var context = new CharacterContext())
                return context.Character.DefaultIfEmpty().Max(s => s.Id);
        }

        public static async Task<List<Model.Character>> GetCharacters(uint accountId)
        {
            using (var context = new CharacterContext())
            {
                return await context.Character
                    .Where(c => c.AccountId == accountId)
                        .Include(c => c.CharacterAppearance)
                        .Include(c => c.CharacterCustomisation)
                    .ToListAsync();
            }
        }

        public static async Task CreateCharacter(Model.Character character)
        {
            using (var context = new CharacterContext())
            {
                context.Character.Add(character);
                await context.SaveChangesAsync();
            }
        }
    }
}
