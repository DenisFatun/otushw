using HomeWorkOTUS.Infrastructure.Services;
using HomeWorkOTUS.Models.Dialogs;
using RestSharp;

namespace HomeWorkOTUS.Services
{
    public class DialogsService : IDialogsService
    {
        public static readonly string HttpClientName = "http_client_dialogs_app";
        private readonly RestClient _restClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DialogsService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            var httpClient = httpClientFactory.CreateClient(HttpClientName);
            _restClient = new RestClient(httpClient);

            var bearerToken = TokenFromHeader();
            if (!string.IsNullOrEmpty(bearerToken))
                _restClient.AddDefaultHeader("Authorization", string.Format("Bearer {0}", bearerToken));            
        }

        public async Task<IEnumerable<Dialog>> ListAsync(Guid from)
        {
            var restRequest = new RestRequest($"v1/dialog/{from}/list", Method.Get);
            var response = await _restClient.ExecuteAsync<IEnumerable<Dialog>>(restRequest);
            return response.Data;
        }

        public async Task SendAsync(Guid toClient, DialogBase dialog)
        {
            var restRequest = new RestRequest($"v1/dialog/{toClient}/send", Method.Post);
            restRequest.AddJsonBody(dialog);
            await _restClient.ExecuteAsync(restRequest);
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
