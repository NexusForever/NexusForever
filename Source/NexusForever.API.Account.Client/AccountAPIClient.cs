namespace NexusForever.API.Account.Client
{
    public class AccountAPIClient : APIClient
    {
        #region Dependency Injection

        public AccountAPIClient(
            HttpClient httpClient)
            : base(httpClient)
        {
        }

        #endregion

        public async Task<Model.Account.Account> GetAccountAsync(uint accountId, CancellationToken token = default)
        {
            return await Get<Model.Account.Account>($"/account/{accountId}", token);
        }

        public async Task<Model.Account.Account> GetAccountAsync(string email, CancellationToken token = default)
        {
            return await Get<Model.Account.Account>($"/account/{email}", token);
        }
    }
}
