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

        private readonly IDatabaseConfig config;

        public CharacterDatabase(IDatabaseConfig config)
        {
            this.config = config;
        }

        public async Task Save(Action<CharacterContext> action)
        {
            await using var context = new CharacterContext(config);
            action.Invoke(context);
            await context.SaveChangesAsync();
        }

        public async Task Save(ISaveCharacter entity)
        {
            await using var context = new CharacterContext(config);
            entity.Save(context);
            await context.SaveChangesAsync();
        }

        public async Task Save(IEnumerable<ISaveCharacter> entities)
        {
            await using var context = new CharacterContext(config);
            foreach (ISaveCharacter entity in entities)
                entity.Save(context);
            await context.SaveChangesAsync();
        }

        public void Migrate()
        {
            using var context = new CharacterContext(config);

            List<string> migrations = context.Database.GetPendingMigrations().ToList();
            if (migrations.Count > 0)
            {
                log.Info($"Applying {migrations.Count} authentication database migration(s)...");
                foreach (string migration in migrations)
                    log.Info(migration);

                context.Database.Migrate();
            }
        }

        public List<CharacterModel> GetAllCharacters()
        {
            using var context = new CharacterContext(config);
            return context.Character.Where(c => c.DeleteTime == null).ToList();
        }

        public ulong GetNextCharacterId()
        {
            using var context = new CharacterContext(config);
            return context.Character
                .Select(r => r.Id)
                .DefaultIfEmpty()
                .Max();
        }

        public async Task<CharacterModel> GetCharacterById(ulong characterId)
        {
            await using var context = new CharacterContext(config);
            return await context.Character.FirstOrDefaultAsync(e => e.Id == characterId);
        }

        public async Task<CharacterModel> GetCharacterByName(string name)
        {
            await using var context = new CharacterContext(config);
            return await context.Character.FirstOrDefaultAsync(e => e.Name == name);
        }

        public ulong GetNextItemId()
        {
            using var context = new CharacterContext(config);
            return context.Item
                .Select(r => r.Id)
                .DefaultIfEmpty()
                .Max();
        }

        public ulong GetNextResidenceId()
        {
            using var context = new CharacterContext(config);
            return context.Residence
                .Select(r => r.Id)
                .DefaultIfEmpty()
                .Max();
        }

        public ulong GetNextDecorId()
        {
            using var context = new CharacterContext(config);
            return context.ResidenceDecor
                .Select(r => r.DecorId)
                .DefaultIfEmpty()
                .Max();
        }

        public async Task<List<CharacterModel>> GetCharacters(uint accountId)
        {
            using var context = new CharacterContext(config);
            return await context.Character.Where(c => c.AccountId == accountId)
                .AsSplitQuery()
                .Include(c => c.Appearance)
                .Include(c => c.Customisation)
                .Include(c => c.Item)
                    .ThenInclude(c => c.Runes)
                .Include(c => c.Bone)
                .Include(c => c.Currency)
                .Include(c => c.Path)
                .Include(c => c.CharacterTitle)
                .Include(c => c.Stat)
                .Include(c => c.Costume)
                    .ThenInclude(c => c.CostumeItem)
                .Include(c => c.PetCustomisation)
                .Include(c => c.PetFlair)
                .Include(c => c.Keybinding)
                .Include(c => c.Spell)
                .Include(c => c.ActionSetShortcut)
                .Include(c => c.ActionSetAmp)
                .Include(c => c.Datacube)
                .Include(c => c.Mail)
                    .ThenInclude(c => c.Attachment)
                        .ThenInclude(c => c.Item)
                            .ThenInclude(c => c.Runes)
                .Include(c => c.ZonemapHexgroup)
                .Include(c => c.Quest)
                    .ThenInclude(c => c.QuestObjective)
                .Include(c => c.Entitlement)
                .Include(c => c.Achievement)
                .Include(c => c.TradeskillMaterials)
                .Include(c => c.Reputation)
                .ToListAsync();
        }

        public bool CharacterNameExists(string characterName)
        {
            using var context = new CharacterContext(config);
            return context.Character.Any(c => c.Name == characterName);
        }

        public List<ResidenceModel> GetResidences()
        {
            using var context = new CharacterContext(config);
            return context.Residence
                .Include(r => r.Plot)
                .Include(r => r.Decor)
                .Include(r => r.Character)
                .Include(r => r.Guild)
                // only load residences where the owner character or guild hasn't been deleted
                .Where(r => (r.OwnerId.HasValue && !r.Character.DeleteTime.HasValue) || (r.GuildOwnerId.HasValue && !r.Guild.DeleteTime.HasValue))
                .ToList();
        }

        public ulong GetNextMailId()
        {
            using var context = new CharacterContext(config);
            return context.CharacterMail
                .Select(r => r.Id)
                .DefaultIfEmpty()
                .Max();
        }

        public ulong GetNextGuildId()
        {
            using var context = new CharacterContext(config);
            return context.Guild
                .Select(r => r.Id)
                .DefaultIfEmpty()
                .Max();
        }

        public List<GuildModel> GetGuilds()
        {
            using var context = new CharacterContext(config);
            return context.Guild
                .Where(g => g.DeleteTime == null)
                .Include(g => g.GuildRank)
                .Include(g => g.GuildMember)
                .Include(g => g.GuildData)
                .Include(g => g.Achievement)
                .ToList();
        }

        public List<ChatChannelModel> GetChatChannels()
        {
            using var context = new CharacterContext(config);
            return context.ChatChannel
                .Include(c => c.Members)
                .ToList();
        }

        public List<CharacterCreateModel> GetCharacterCreationData()
        {
            using var context = new CharacterContext(config);

            return context.CharacterCreate.ToList();
        }
    }
}
