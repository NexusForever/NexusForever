using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Character.Model;
using NexusForever.Database.Configuration;
using NLog;

namespace NexusForever.Database.Character
{
    public class CharacterDatabase
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly IDatabaseConfiguration config;

        public CharacterDatabase(IDatabaseConfiguration config)
        {
            this.config = config;
        }

        public void Migrate()
        {
            using (var context = new CharacterContext(config))
            {
                List<string> migrations = context.Database.GetPendingMigrations().ToList();
                if (migrations.Count > 0)
                {
                    log.Info($"Applying {migrations.Count} character database migration(s)...");
                    foreach (string migration in migrations)
                        log.Info(migration);

                    context.Database.Migrate();
                }
            }
        }

        public async Task Save(Action<CharacterContext> action)
        {
            using (var context = new CharacterContext(config))
            {
                action.Invoke(context);
                await context.SaveChangesAsync();
            }
        }

        public ulong GetNextCharacterId()
        {
            using (var context = new CharacterContext(config))
                return context.Character.DefaultIfEmpty().Max(s => s.Id);
        }

        public Task<CharacterModel> GetCharacterById(ulong characterId)
        {
            using (var context = new CharacterContext(config))
                return context.Character.FirstOrDefaultAsync(e => e.Id == characterId);
        }

        public Task<CharacterModel> GetCharacterByName(string name)
        {
            using (var context = new CharacterContext(config))
                return context.Character.FirstOrDefaultAsync(e => e.Name == name);
        }

        public ulong GetNextItemId()
        {
            using (var context = new CharacterContext(config))
                return context.Item.DefaultIfEmpty().Max(s => s.Id);
        }

        public ulong GetNextResidenceId()
        {
            using (var context = new CharacterContext(config))
                return context.Residence.DefaultIfEmpty().Max(r => r.Id);
        }

        public ulong GetNextDecorId()
        {
            using (var context = new CharacterContext(config))
                return context.ResidenceDecor.DefaultIfEmpty().Max(r => r.DecorId);
        }

        public async Task<List<CharacterModel>> GetCharacters(uint accountId)
        {
            using (var context = new CharacterContext(config))
            {
                return await context.Character
                    .Where(c => c.AccountId == accountId)
                        .Include(c => c.Appearance)
                        .Include(c => c.Customisations)
                        .Include(c => c.Items)
                        .Include(c => c.Bones)
                        .Include(c => c.Currencies)
                        .Include(c => c.Paths)
                        .Include(c => c.Titles)
                        .Include(c => c.Stats)
                        .Include(c => c.Costumes)
                            .ThenInclude(c => c.Items)
                        .Include(c => c.PetCustomisations)
                        .Include(c => c.PetFlairs)
                        .Include(c => c.Keybindings)
                        .Include(c => c.Spells)
                        .Include(c => c.ActionSetShortcuts)
                        .Include(c => c.ActionSetAmps)
                        .Include(c => c.Datacubes)
                        .Include(c => c.Mail)
                            .ThenInclude(c => c.Attachments)
                                .ThenInclude(a => a.Item)
                        .Include(c => c.ZonemapHexgroups)
                        .Include(c => c.Quests)
                            .ThenInclude(q => q.Objectives)
                        .Include(c => c.Entitlements)
                        .Include(c => c.Achievements)
                    .ToListAsync();
            }
        }

        public bool CharacterNameExists(string characterName)
        {
            using (var context = new CharacterContext(config))
                return context.Character.Any(c => c.Name == characterName);
        }

        public async Task<ResidenceModel> GetResidence(ulong residenceId)
        {
            using (var context = new CharacterContext(config))
            {
                return await context.Residence
                    .Include(r => r.Decor)
                    .Include(r => r.Plots)
                    .Include(r => r.Owner)
                    .SingleOrDefaultAsync(r => r.Id == residenceId);
            }
        }

        public async Task<ResidenceModel> GetResidence(string name)
        {
            using (var context = new CharacterContext(config))
            {
                return await context.Residence
                    .Include(r => r.Decor)
                    .Include(r => r.Plots)
                    .Include(r => r.Owner)
                    .SingleOrDefaultAsync(r => r.Owner.Name == name);
            }
        }

        public List<ResidenceModel> GetPublicResidences()
        {
            using (var context = new CharacterContext(config))
            {
                return context.Residence
                    .Include(r => r.Owner)
                    .Where(r => r.PrivacyLevel == 0)
                    .ToList();
            }
        }

        public ulong GetNextMailId()
        {
            using (var context = new CharacterContext(config))
                return context.Mail.DefaultIfEmpty().Max(s => s.Id);
        }
    }
}
