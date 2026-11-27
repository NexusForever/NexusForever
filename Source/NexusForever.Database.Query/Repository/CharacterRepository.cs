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

        public async Task<CharacterModel> GetOrInsertCharacterAsync(CharacterModel character)
        {
            // Concurrent handlers can both miss the cache. Keep the row the first one inserted.
            await _context.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO `character`
                    (`characterId`, `realmId`, `name`, `realmName`, `race`, `class`, `path`, `faction`, `sex`,
                     `currentRealmId`, `worldZoneId`, `level`, `guildName`, `lastOnline`)
                VALUES
                    ({character.CharacterId}, {character.RealmId}, {character.Name}, {character.RealmName},
                     {(byte)character.Race}, {(byte)character.Class}, {(byte)character.Path}, {(uint)character.Faction},
                     {(byte)character.Sex}, {character.CurrentRealmId}, {character.WorldZoneId}, {character.Level},
                     {character.GuildName}, {character.LastOnline})
                ON DUPLICATE KEY UPDATE `characterId` = `characterId`
                """);

            return await GetCharacterAsync(character.CharacterId, character.RealmId);
        }

        public async Task<CharacterModel> GetCharacterAsync(ulong characterId, uint realmId)
        {
            return await _context.Character.SingleOrDefaultAsync(c => c.CharacterId == characterId && c.RealmId == realmId);
        }
    }
}
