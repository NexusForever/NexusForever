using Microsoft.Extensions.DependencyInjection;
using NexusForever.API.Character.Client;
using NexusForever.Database.Chat.Model;
using NexusForever.Database.Chat.Repository;

namespace NexusForever.Server.ChatServer.Character
{
    public class CharacterManager
    {
        private readonly Dictionary<Identity, Character> _characterCache = [];
        private readonly Dictionary<IdentityName, Character> _characterNameCache = [];

        #region Dependency Injection

        private readonly CharacterRepository _repository;
        private readonly CharacterAPIClient _apiClient;
        private readonly IServiceProvider _serviceProvider;

        public CharacterManager(
            CharacterRepository repository,
            CharacterAPIClient apiClient,
            IServiceProvider serviceProvider)
        {
            _repository      = repository;
            _apiClient       = apiClient;
            _serviceProvider = serviceProvider;
        }

        #endregion

        /// <summary>
        /// Add a character to the local database.
        /// </summary>
        /// <param name="character">Character model to save to local character database.</param>
        public Character AddCharacter(CharacterModel character)
        {
            _repository.AddCharacter(character);
            return InitialiseCharacter(character);
        }

        /// <summary>
        /// Get a character.
        /// </summary>
        /// <remarks>
        /// If the character does not exist in the database it will not be fetched from the API.
        /// </remarks>
        /// <param name="identity">Identity of the character to return.</param>
        public async Task<Character> GetCharacterAsync(Identity identity)
        {
            if (_characterCache.TryGetValue(identity, out Character character))
                return character;

            CharacterModel characterModel = await _repository.GetCharacterAsync(identity.Id, identity.RealmId);
            if (characterModel == null)
                return null;

            return InitialiseCharacter(characterModel);
        }

        /// <summary>
        /// Get a character.
        /// </summary>
        /// <remarks>
        /// If the character does not exist in the database it will be fetched from the API and saved to the local database.
        /// </remarks>
        /// <param name="identity">Identity of the character to return.</param>
        public async Task<Character> GetCharacterRemoteAsync(Identity identity)
        {
            if (_characterCache.TryGetValue(identity, out Character character))
                return character;

            CharacterModel databaseModel = await _repository.GetCharacterAsync(identity.Id, identity.RealmId);
            if (databaseModel != null)
                return InitialiseCharacter(databaseModel);

            API.Model.Character.Character apiModel = await _apiClient.GetCharacterAsync(identity.ToAPIdentity());
            if (apiModel == null)
                return null;

            return AddCharacter(apiModel.ToDatabaseCharacter());
        }

        /// <summary>
        /// Get a character.
        /// </summary>
        /// <remarks>
        /// If the character does not exist in the database it will be fetched from the API and saved to the local database.
        /// </remarks>
        /// <param name="identity">Identity of the character to return.</param>
        public async Task<Character> GetCharacterRemoteAsync(IdentityName identity)
        {
            if (_characterNameCache.TryGetValue(identity, out Character character))
                return character;

            CharacterModel databaseModel = await _repository.GetCharacterAsync(identity.Name, identity.RealmName);
            if (databaseModel != null)
                return InitialiseCharacter(databaseModel);

            API.Model.Character.Character apiModel = await _apiClient.GetCharacterAsync(identity.ToAPIIdentity());
            if (apiModel == null)
                return null;

            return AddCharacter(apiModel.ToDatabaseCharacter());
        }

        private Character InitialiseCharacter(CharacterModel model)
        {
            var character = _serviceProvider.GetRequiredService<Character>();
            character.Initialise(model);
            _characterCache.Add(character.Identity, character);
            _characterNameCache.Add(character.IdentityName, character);
            return character;
        }
    }
}
