using CommonLib.Infrastructure.Services;

namespace CountsApp.Infrastructure.Services
{
    public interface IRedisService : IService
    {
        Task SaveAsync(string key, int[] values);
        Task<int[]> GetAsync(string key);
    }
}
