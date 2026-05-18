using System.Net.Http.Json;
using NexusForever.API.Model;

namespace NexusForever.API
{
    public class APIClient
    {
        #region Dependency Injection

        private readonly HttpClient _httpClient;

        public APIClient(
            HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #endregion

        public async Task<T> Get<T>(string url, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage result = await _httpClient.GetAsync(url, cancellationToken);
            if (result.IsSuccessStatusCode)
                return await result.Content.ReadFromJsonAsync<T>(cancellationToken);
            else
            {
                if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return default;

                if (result.Content.Headers.ContentType?.MediaType == "application/problem+json")
                {
                    var problemDetails = await result.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);
                    throw new HttpRequestException($"Error: {problemDetails?.Title} - {problemDetails?.Detail}");
                }

                throw new HttpRequestException();
            }
        }
    }
}
