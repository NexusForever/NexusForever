using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Chat.Model;

namespace NexusForever.Database.Chat.Repository
{
    public class CharacterRepository
    {
        private readonly ChatContext _context;

        public CharacterRepository(
            ChatContext context)
        {
            _context = context;
        }

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
            return await _context.Character
                .Include(c => c.Channels)
                .SingleOrDefaultAsync(c => c.CharacterId == id && c.RealmId == realmId);
        }

        public async Task<CharacterModel> GetCharacterAsync(string name, string realmName)
        {
            return await _context.Character
                .Include(c => c.Channels)
                .SingleOrDefaultAsync(c => c.Name == name && c.RealmName == realmName);
        }
    }
}
