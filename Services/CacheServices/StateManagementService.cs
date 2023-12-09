using JobHubBot.Interfaces.IDbInterfaces;
using JobHubBot.Services.Enums;
using System.Collections.Concurrent;

namespace JobHubBot.Services.CacheServices
{
    public class StateManagementService : IStateManagementService
    {
        private static readonly ConcurrentDictionary<long, StateList> KeyValuesState = new();

        public Task SetUserState(long chatId, StateList state)
        {
            KeyValuesState[chatId] = state;
            return Task.CompletedTask;
        }

        public Task DeleteState(long Id)
        {
            KeyValuesState.TryRemove(Id, out _);
            return Task.CompletedTask;
        }

        public StateList? GetUserState(long Id)
        {
            KeyValuesState.TryGetValue(Id, out var state);
            return state;
        }
    }

}
