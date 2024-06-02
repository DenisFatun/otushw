using DialogsApp.Infrastructure.Services;
using RestSharp;

namespace DialogsApp.Services
{
    public class CountsService : ICountsService
    {
        public static readonly string HttpClientName = "http_client_counts_app";
        private readonly RestClient _restClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CountsService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            var httpClient = httpClientFactory.CreateClient(HttpClientName);
            _restClient = new RestClient(httpClient);

            var bearerToken = TokenFromHeader();
            if (!string.IsNullOrEmpty(bearerToken))
                _restClient.AddDefaultHeader("Authorization", string.Format("Bearer {0}", bearerToken));
        }

        public async Task CreateAsync(Guid to, Guid from, int dialogId)
        {
            var restRequest = new RestRequest($"v1/counts", Method.Post);
            restRequest.AddJsonBody(new { to, from, dialogId });
            var response = await _restClient.ExecuteAsync(restRequest);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);
        }

        public async Task UpdateAsync(Guid to, Guid from, int lastReadId)
        {
            var restRequest = new RestRequest($"v1/counts", Method.Put);
            restRequest.AddJsonBody(new { to, from, lastReadId});
            var response = await _restClient.ExecuteAsync(restRequest);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);
        }

        private string TokenFromHeader()
        {
            string header = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            return (!string.IsNullOrEmpty(header))
                ? header.Replace("Bearer ", "")
                : string.Empty;
        }
    }
}
