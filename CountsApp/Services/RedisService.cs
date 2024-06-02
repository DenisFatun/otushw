using CountsApp.Infrastructure.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace CountsApp.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;
        public RedisService(IDatabase database)
        {
            _database = database;
        }

        public async Task SaveAsync(string key, int[] values)
        {            
            string value = JsonSerializer.Serialize(values);
            await _database.StringSetAsync(key, value);            
        }

        public async Task<int[]> GetAsync(string key)
        {            
            var value = await _database.StringGetAsync(key);
            if (string.IsNullOrEmpty(value)) 
                return null;

            return JsonSerializer.Deserialize<int[]>(value);
        }
    }
}
