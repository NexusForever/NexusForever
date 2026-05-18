using Microsoft.Extensions.DependencyInjection;
using NexusForever.API.Character.Client;
using NexusForever.Database.Group.Model;
using NexusForever.Database.Group.Repository;

namespace NexusForever.Server.GroupServer.Character
{
    public class CharacterManager
    {
        private readonly Dictionary<Identity, Character> _characterCache = [];
        private readonly Dictionary<IdentityName, Character> _characterNameCache = [];

        #region Dependency Injection

        private readonly CharacterRepository _characterRepository;
        private readonly CharacterAPIClient _characterApiClient;
        private readonly IServiceProvider _serviceProvider;

        public CharacterManager(
            CharacterRepository characterRepository,
            CharacterAPIClient characterApiClient,
            IServiceProvider serviceProvider)
        {
            _characterRepository = characterRepository;
            _characterApiClient = characterApiClient;
            _serviceProvider = serviceProvider;
        }

        #endregion

        /// <summary>
        /// Get a character.
        /// </summary>
        /// <remarks>
        /// If the character does not exist in the database it will not be fetched from the API.
        /// </remarks>
        /// <param name="identity">Identity of the character to return.</param>
        public async Task<Character> GetCharacterAsync(Identity identity)
        {
            if (_characterCache.TryGetValue(identity, out Character cachedCharacter))
                return cachedCharacter;

            CharacterModel characterModel = await _characterRepository.GetCharacterAsync(identity.Id, identity.RealmId);
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
            if (_characterCache.TryGetValue(identity, out Character cachedCharacter))
                return cachedCharacter;

            CharacterModel characterModel = await _characterRepository.GetCharacterAsync(identity.Id, identity.RealmId);
            if (characterModel == null)
            {
                API.Model.Character.Character apiCharacter = await _characterApiClient.GetCharacterAsync(identity.ToAPIdentity());
                if (apiCharacter == null)
                    return null;

                characterModel = apiCharacter.ToDatabaseCharacter();
                _characterRepository.AddCharacter(characterModel);
            }

            return InitialiseCharacter(characterModel);
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
            if (_characterNameCache.TryGetValue(identity, out Character cachedCharacter))
                return cachedCharacter;

            CharacterModel characterModel = await _characterRepository.GetCharacterAsync(identity.Name, identity.RealmName);
            if (characterModel == null)
            {
                API.Model.Character.Character apiCharacter = await _characterApiClient.GetCharacterAsync(identity.ToAPIdentityName());
                if (apiCharacter == null)
                    return null;

                characterModel = apiCharacter.ToDatabaseCharacter();
                _characterRepository.AddCharacter(characterModel);
            }

            return InitialiseCharacter(characterModel);
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
