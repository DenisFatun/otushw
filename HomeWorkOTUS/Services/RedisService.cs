using HomeWorkOTUS.Handlers;
using HomeWorkOTUS.Infrastructure.Services;
using HomeWorkOTUS.Models;
using MassTransit.Initializers;
using StackExchange.Redis;

namespace HomeWorkOTUS.Services
{    
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;
        private readonly long _listLimit;

        public RedisService(IDatabase database, IConfiguration config)
        {
            _database = database;
            _listLimit = Convert.ToInt64(config["Redis:ListLimit"]);
        }

        public async Task<IEnumerable<string>> ListAsync(string key, OffsetFilter filter)
        {
            var result = await _database.ListRangeAsync(key, filter.Offset, filter.Offset + filter.Limit -1);
            return result.Select(x=>x.ToString());
        }

        public async Task SaveListAsync(string key, string value)
        {
            var values = new RedisValue[] { new RedisValue(value) };
            await _database.ListLeftPushAsync(key, values);
            var length = await _database.ListLengthAsync(key);
            if (length > _listLimit)
                await _database.ListTrimAsync(key, 0, _listLimit - 1);          
        }

        public async Task RemoveListAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }
    }
}
