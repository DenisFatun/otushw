using HomeWorkOTUS.Models.Posts;

namespace HomeWorkOTUS.Infrastructure.Services
{
    public interface IPostsService : IService
    {
        Task AddPostAsync(Guid clientId, string text);
        Task RemovePostAsync(Guid clientId, int postId);
        Task UpdatePostAsync(Guid clientId, int postId, string text);
        Task<ClientPost> GetPostAsync(int postId);
        Task<IEnumerable<ClientPost>> ListPostFromCacheAsync(Guid clientId, PostListFilter filter);
        Task<IEnumerable<ClientPost>> ListPostAsync(Guid clientId, PostListFilter filter);
        Task<int> UpdatePostCacheAsync(List<Guid> clientIdList);
        Task<IEnumerable<ClientPostSimple>> PostsByAuthorAsync(Guid clientId);
        Task AddPostSimpleAsync(Guid clientId, string text);
    }
}
