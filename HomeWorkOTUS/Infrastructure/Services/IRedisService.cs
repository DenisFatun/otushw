using HomeWorkOTUS.Models;
using HomeWorkOTUS.Models.Posts;

namespace HomeWorkOTUS.Infrastructure.Services
{
    public interface IRedisService : IService
    {
        Task<IEnumerable<string>> ListAsync(string key, OffsetFilter filter);
        Task SaveListAsync(string key, string value);
        Task RemoveListAsync(string key);
    }
}
