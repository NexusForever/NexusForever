using NexusForever.API.Character.Database;
using NexusForever.API.Character.Server;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Database.Character.Repository;

namespace NexusForever.API.Character.Character
{
    public class CharacterManager
    {
        #region Dependency Injection

        private readonly ServerManager _serverManager;
        private readonly ContextFactory<CharacterContext> _contextFactory;

        public CharacterManager(
            ServerManager serverManager,
            ContextFactory<CharacterContext> contextFactory)
        {
            _serverManager  = serverManager;
            _contextFactory = contextFactory;
        }

        #endregion

        /// <summary>
        /// Get a character from the database.
        /// </summary>
        /// <param name="realmId">Realm id of the character.</param>
        /// <param name="characterId">Id of the character.</param>
        public async Task<Model.Character.Character> GetCharacterAsync(ushort realmId, ulong characterId)
        {
            Server.Server server = await _serverManager.GetServerAsync(realmId);
            if (server == null)
                return null;

            CharacterContext context = _contextFactory.GetContext(server.Id);
            var repository = new CharacterRepository(context);

            CharacterModel character = await repository.GetCharacterAsync(characterId);
            return character?.ToCharacter(server);
        }

        /// <summary>
        /// Get a character from the database.
        /// </summary>
        /// <param name="realmName">Realm name of the character.</param>
        /// <param name="characterName">Name of the character.</param>
        public async Task<Model.Character.Character> GetCharacterAsync(string realmName, string characterName)
        {
            Server.Server server = await _serverManager.GetServerAsync(realmName);
            if (server == null)
                return null;

            CharacterContext context = _contextFactory.GetContext(server.Id);
            var repository = new CharacterRepository(context);

            CharacterModel character = await repository.GetCharacterAsync(characterName);
            return character?.ToCharacter(server);
        }
    }
}
