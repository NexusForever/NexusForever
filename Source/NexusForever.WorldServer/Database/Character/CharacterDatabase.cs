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

        public static List<Model.Character> GetAllCharacters()
        {
            using (var context = new CharacterContext())
                return context.Character.Where(c => c.DeleteTime == null).ToList();
        }

        public static ulong GetNextCharacterId()
        {
            using (var context = new CharacterContext())
                return context.Character.DefaultIfEmpty().Max(s => s.Id);
        }

        public static async Task<Model.Character> GetCharacterById(ulong characterId)
        {
            using (var context = new CharacterContext())
                return await context.Character.FirstOrDefaultAsync(e => e.Id == characterId);
        }

        public static async Task<Model.Character> GetCharacterByName(string name)
        {
            using (var context = new CharacterContext())
                return await context.Character.FirstOrDefaultAsync(e => e.Name == name);
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
                        .Include(c => c.CharacterStats)
                        .Include(c => c.CharacterCostume)
                            .ThenInclude(c => c.CharacterCostumeItem)
                        .Include(c => c.CharacterPetCustomisation)
                        .Include(c => c.CharacterPetFlair)
                        .Include(c => c.CharacterKeybinding)
                        .Include(c => c.CharacterSpell)
                        .Include(c => c.CharacterActionSetShortcut)
                        .Include(c => c.CharacterActionSetAmp)
                        .Include(c => c.CharacterDatacube)
                        .Include(c => c.CharacterMail)
                            .ThenInclude(c => c.CharacterMailAttachment)
                                .ThenInclude(a => a.ItemGu)
                        .Include(c => c.CharacterZonemapHexgroup)
                        .Include(c => c.CharacterQuest)
                            .ThenInclude(q => q.CharacterQuestObjective)
                        .Include(c => c.CharacterEntitlement)
                        .Include(c => c.CharacterAchievement)
                    .ToListAsync();
            }
        }

        public static bool CharacterNameExists(string characterName)
        {
            using (var context = new CharacterContext())
                return context.Character.Any(c => c.Name == characterName);
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

        public static async Task<Residence> GetResidence(ulong residenceId)
        {
            using (var context = new CharacterContext())
            {
                return await context.Residence
                    .Include(r => r.ResidenceDecor)
                    .Include(r => r.ResidencePlot)
                    .Include(r => r.Owner)
                    .SingleOrDefaultAsync(r => r.Id == residenceId);
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

        public static List<Residence> GetPublicResidences()
        {
            using (var context = new CharacterContext())
            {
                return context.Residence
                    .Include(r => r.Owner)
                    .Where(r => r.PrivacyLevel == 0)
                    .ToList();
            }
        }

        public static ulong GetNextMailId()
        {
            using (var context = new CharacterContext())
                return context.CharacterMail.DefaultIfEmpty().Max(s => s.Id);
        }
    }
}
