using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using ItemEntity = NexusForever.WorldServer.Game.Entity.Item;

namespace NexusForever.WorldServer.Database.Character
{
    public static class CharacterDatabase
    {
        public static ulong GetNextCharacterId()
        {
            using (var context = new CharacterContext())
                return context.Character.DefaultIfEmpty().Max(s => s.Id);
        }

        public static ulong GetNextItemId()
        {
            using (var context = new CharacterContext())
                return context.Item.DefaultIfEmpty().Max(s => s.Id);
        }

        public static Model.Item GetItemById(ulong Id)
        {
            using (var context = new CharacterContext())
                return context.Item.Where(e => e.Id == Id).First();
        }

        public static async Task<List<Model.Character>> GetCharacters(uint accountId)
        {
            using (var context = new CharacterContext())
            {
                return await context.Character
                    .Where(c => c.AccountId == accountId)
                        .Include(c => c.CharacterAppearance)
                        .Include(c => c.CharacterCustomisation)
                        .Include(c => c.Item)
                        .Include(c => c.CharacterBone)
                        .Include(c => c.CharacterCurrency)
                        .Include(c => c.CharacterPath)
                        .Include(c => c.CharacterTitle)
                    .ToListAsync();
            }
        }

        public static async Task CreateCharacter(Model.Character character, IEnumerable<ItemEntity> items)
        {
            using (var context = new CharacterContext())
            {
                context.Character.Add(character);
                foreach (var item in items)
                    item.Save(context);

                await context.SaveChangesAsync();
            }
        }

        public static async Task SavePlayer(Player player)
        {
            using (var context = new CharacterContext())
            {
                player.Save(context);
                await context.SaveChangesAsync();
            }
        }
    }
}
