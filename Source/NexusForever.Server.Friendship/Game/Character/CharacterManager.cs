using Microsoft.Extensions.DependencyInjection;
using NexusForever.API.Character.Client;
using NexusForever.Database.Friendship.Model;
using NexusForever.Database.Friendship.Repository;
using NexusForever.Server.Friendship.Game.Account;

namespace NexusForever.Server.Friendship.Game.Character
{
    public class CharacterManager
    {
        // scoped character cache to prevent multiple database calls for the same character during a request
        private readonly Dictionary<Identity, Character> _characterCache = [];
        private readonly Dictionary<IdentityName, Character> _characterNameCache = [];

        #region Dependency Injection

        private readonly CharacterRepository _repository;
        private readonly CharacterAPIClient _apiClient;
        private readonly AccountManager _accountManager;
        private readonly IServiceProvider _serviceProvider;

        public CharacterManager(
            CharacterRepository repository,
            CharacterAPIClient apiClient,
            AccountManager accountManager,
            IServiceProvider serviceProvider)
        {
            _repository      = repository;
            _apiClient       = apiClient;
            _accountManager  = accountManager;
            _serviceProvider = serviceProvider;
        }

        #endregion


        /// <summary>
        /// Get an <see cref="Character"/> with the specified character identity.
        /// </summary>
        /// <remarks>
        /// If the character does not exist in the database it will not be fetched from the API.
        /// A local cache is used to prevent multiple database calls for the same character during a single request.
        /// </remarks>
        /// <param name="identity">Identity of the <see cref="Character"/> to return.</param>
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
        /// Get an <see cref="Character"/> with the specified character identity.
        /// </summary>
        /// <remarks>
        /// If the character does not exist in the database it will be fetched from the API and saved to the local database.
        /// A local cache is used to prevent multiple database calls for the same character during a single request.
        /// </remarks>
        /// <param name="identity">Identity of the <see cref="Character"/> to return.</param>
        public async Task<Character> GetCharacterRemoteAsync(Identity identity)
        {
            if (_characterCache.TryGetValue(identity, out Character character))
                return character;

            CharacterModel databaseModel = await _repository.GetCharacterAsync(identity.Id, identity.RealmId);
            if (databaseModel == null)
            {
                API.Model.Character.Character apiModel = await _apiClient.GetCharacterAsync(identity.ToAPIdentity());
                if (apiModel == null)
                    return null;

                Account.Account account = await _accountManager.GetAccountAsync(apiModel.AccountId);
                if (account == null)
                    return null;

                databaseModel = apiModel.ToDatabaseCharacter();
                _repository.AddCharacter(databaseModel);
            }

            return InitialiseCharacter(databaseModel);
        }

        /// <summary>
        /// Get an <see cref="Character"/> with the specified character name identity.
        /// </summary>
        /// <remarks>
        /// If the character does not exist in the database it will be fetched from the API and saved to the local database.
        /// A local cache is used to prevent multiple database calls for the same character during a single request.
        /// </remarks>
        /// <param name="identity">Name identity of the <see cref="Character"/> to return.</param>
        public async Task<Character> GetCharacterRemoteAsync(IdentityName identity)
        {
            if (_characterNameCache.TryGetValue(identity, out Character character))
                return character;

            CharacterModel characterModel = await _repository.GetCharacterAsync(identity.Name, identity.RealmName);
            if (characterModel == null)
            {
                API.Model.Character.Character apiCharacter = await _apiClient.GetCharacterAsync(identity.ToAPIdentityName());
                if (apiCharacter == null)
                    return null;

                Account.Account account = await _accountManager.GetAccountAsync(apiCharacter.AccountId);
                if (account == null)
                    return null;

                characterModel = apiCharacter.ToDatabaseCharacter();
                _repository.AddCharacter(characterModel);
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
