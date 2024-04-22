using CommonLib.Data;
using CommonLib.Infrastructure.Repos;
using HomeWorkOTUS.Models.Posts;

namespace HomeWorkOTUS.Infrastructure.Repos
{
    public interface IPostsRepo : IRepo
    {
        Task<int> CreateAsync(Guid clientId, string text);
        Task DeleteAsync(Guid clientId, int id);
        Task UpdateAsync(Guid clientId, int id, string text);
        Task<ClientPost> GetAsync(int id);
        Task<IEnumerable<ClientPost>> ListAsync(Guid clientId, PostListFilter filter, SqlOrder order = SqlOrder.DESC);
        Task<IEnumerable<ClientPostSimple>> PostsByAuthorAsync(Guid clientId);
    }
}
