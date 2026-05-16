using Microsoft.Extensions.DependencyInjection;
using NexusForever.API.Character.Client;
using NexusForever.Database.Query.Model;
using NexusForever.Database.Query.Repository;

namespace NexusForever.Server.Character.Game.Character
{
    public class CharacterManager
    {
        #region Dependency Injection

        private readonly IServiceProvider _serviceProvider;
        private readonly CharacterRepository _repository;
        private readonly CharacterAPIClient _apiClient;

        public CharacterManager(
            IServiceProvider serviceProvider,
            CharacterRepository repository,
            CharacterAPIClient apiClient)
        {
            _serviceProvider = serviceProvider;
            _repository      = repository;
            _apiClient       = apiClient;
        }

        #endregion

        /// <summary>
        /// Get an <see cref="Character"/> with the specified character identity.
        /// </summary>
        /// <remarks>
        /// If the character does not exist in the database it will be fetched from the API and saved to the local database.
        /// </remarks>
        /// <param name="identity">Identity of the <see cref="Character"/> to return.</param>
        public async Task<Character> GetCharacterRemoteAsync(Identity identity)
        {
            CharacterModel databaseModel = await _repository.GetCharacterAsync(identity.Id, identity.RealmId);
            if (databaseModel == null)
            {
                API.Model.Character.Character apiModel = await _apiClient.GetCharacterAsync(identity.ToAPIIdentity());
                if (apiModel == null)
                    return null;

                databaseModel = apiModel.ToDatabaseCharacter();
                _repository.AddCharacter(databaseModel);
            }

            var character = _serviceProvider.GetRequiredService<Character>();
            character.Initialise(databaseModel);
            return character;
        }
    }
}
