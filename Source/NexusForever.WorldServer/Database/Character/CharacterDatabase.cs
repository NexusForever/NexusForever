using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NexusForever.WorldServer.Database.Character.Model;
using ItemEntity = NexusForever.WorldServer.Game.Entity.Item;
using ResidenceEntity = NexusForever.WorldServer.Game.Housing.Residence;

namespace NexusForever.WorldServer.Database.Character
{
    public static class CharacterDatabase
    {
        public static async Task Save(Action<CharacterContextExtended> action)
        {
            using (var context = new CharacterContextExtended())
            {
                action.Invoke(context);
                await context.SaveChangesAsync();
            }
        }

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

        public static ulong GetNextResidenceId()
        {
            using (var context = new CharacterContext())
                return context.Residence.DefaultIfEmpty().Max(r => r.Id);
        }

        public static ulong GetNextDecorId()
        {
            using (var context = new CharacterContext())
                return context.ResidenceDecor.DefaultIfEmpty().Max(r => r.DecorId);
        }

        public static Model.Item GetItemById(ulong id)
        {
            using (var context = new CharacterContext())
                return context.Item.First(e => e.Id == id);
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
                        .Include(c => c.CharacterCostume)
                            .ThenInclude(c => c.CharacterCostumeItem)
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

        public static async Task SaveResidence(ResidenceEntity residence)
        {
            using (var context = new CharacterContext())
            {
                residence.Save(context);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<Residence> GetResidence(string name)
        {
            using (var context = new CharacterContext())
            {
                return await context.Residence
                    .Include(r => r.ResidenceDecor)
                    .Include(r => r.ResidencePlot)
                    .Include(r => r.Owner)
                    .SingleOrDefaultAsync(r => r.Owner.Name == name);
            }

        }
    }
}
