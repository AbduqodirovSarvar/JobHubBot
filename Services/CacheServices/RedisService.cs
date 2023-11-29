/*using IDatabase = StackExchange.Redis.IDatabase;
using JobHubBot.Models.Telegram;

namespace JobHubBot.Services.CacheServices
{
    public class RedisService
    {
        private readonly IDatabase _redisDb;
        public RedisService(IDatabase redis)
        {
            _redisDb = redis;
        }

        public async Task SetUserState(long chatId, string state)
        {
            await _redisDb.StringSetAsync($"{chatId}", state);
        }

        public async Task<string?> GetUserState(long chatId)
        {
            string? state = await _redisDb.StringGetAsync($"{chatId}");
            return state;
        }

        public Task DeleteState(long Id)
        {
            _redisDb.KeyDelete($"{Id}");

            return Task.CompletedTask;
        }

        public async Task Next(long Id)
        {
            string? state = await _redisDb.StringGetAsync($"{Id}");
            if (state == null)
            {
                return;
            }
            await _redisDb.StringSetAsync($"{Id}", States.states[Array.IndexOf(States.states, state) + 1]);

            var a = await _redisDb.StringGetAsync($"{Id}");

            Console.WriteLine("STATE:_____________________" + a);

            return;
        }
    }
}
*/