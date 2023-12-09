using JobHubBot.Interfaces.IDbInterfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using IDatabase = StackExchange.Redis.IDatabase;

namespace JobHubBot.Services.CacheServices
{
    public class RedisService : ICacheDbService
    {
        private readonly IDatabase _redisDb;

        public RedisService(IConnectionMultiplexer connectionMultiplexer)
        {
            _redisDb = connectionMultiplexer.GetDatabase();
        }

        public async Task DeleteAsync(string key)
        {
            await _redisDb.KeyDeleteAsync(key);
        }

        public async Task<T?> GetObjectAsync<T>(string key) where T : class
        {
            RedisValue value = await _redisDb.StringGetAsync(key);
            if (value.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(value!);
            }
            return null;
        }

        public async Task SetObjectAsync<T>(string key, T obj) where T : class
        {
            var stringJson = JsonConvert.SerializeObject(obj);
            await _redisDb.StringSetAsync(key, stringJson);
            await _redisDb.KeyExpireAsync(key, TimeSpan.FromDays(1));
        }

    }
}
