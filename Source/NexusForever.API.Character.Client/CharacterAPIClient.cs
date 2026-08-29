using NexusForever.API.Model;

namespace NexusForever.API.Character.Client
{
    public class CharacterAPIClient : APIClient
    {
        #region Dependency Injection

        public CharacterAPIClient(
            HttpClient httpClient)
            : base(httpClient)
        {
        }

        #endregion

        public async Task<Model.Character.Character> GetCharacterAsync(Identity identity, CancellationToken cancellationToken = default)
        {
            return await Get<Model.Character.Character>($"/character/{identity.RealmId}/{identity.Id}", cancellationToken);
        }

        public async Task<Model.Character.Character> GetCharacterAsync(IdentityName identity, CancellationToken cancellationToken = default)
        {
            return await Get<Model.Character.Character>($"/character/{identity.RealmName}/{identity.Name}", cancellationToken);
        }
    }
}
