using HomeWorkOTUS.Infrastructure.Repos;
using HomeWorkOTUS.Models.Posts;
using MassTransit;
using RestSharp;

namespace HomeWorkOTUS.Data.Repos
{
    public class PostsRepoTarantool : IPostsRepo
    {
        public static readonly string HttpClientName = "http_client_tarantool";
        private readonly RestClient _restClient;

        public PostsRepoTarantool(IHttpClientFactory httpClientFactory)
        {
            var httpClient = httpClientFactory.CreateClient(HttpClientName);
            _restClient = new RestClient(httpClient);
        }

        public async Task<int> CreateAsync(Guid clientId, string text)
        {
            var restRequest = new RestRequest("posts", Method.Post);
            restRequest.AddJsonBody(new
            {
                author = clientId,
                post_text = text,
                created_at = DateTime.Now
            });
            await _restClient.ExecuteAsync(restRequest);
            return 0;
        }

        public Task DeleteAsync(Guid clientId, int id)
        {
            throw new NotImplementedException();
        }

        public Task<ClientPost> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ClientPost>> ListAsync(Guid clientId, PostListFilter filter, SqlOrder order = SqlOrder.DESC)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ClientPostSimple>> PostsByAuthorAsync(Guid clientId)
        {
            var restRequest = new RestRequest($"posts/{clientId}", Method.Get);
            var response = await _restClient.ExecuteAsync<IEnumerable<ClientPostSimple>>(restRequest);
            return response.Data;
        }

        public Task UpdateAsync(Guid clientId, int id, string text)
        {
            throw new NotImplementedException();
        }
    }
}
