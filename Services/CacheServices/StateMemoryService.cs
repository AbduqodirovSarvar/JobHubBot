namespace JobHubBot.Services.CacheServices
{
    public class StateMemoryService
    {
        private static Dictionary<string, string> KeyValuesState = new();

        public Task SetUserState(long chatId, string state)
        {
            if (KeyValuesState.ContainsKey($"{chatId}"))
            {
                KeyValuesState[$"{chatId}"] = state;
                return Task.CompletedTask;
            }

            KeyValuesState.Add($"{chatId}", state);
            return Task.CompletedTask;
        }

        public Task DeleteState(long Id)
        {
            if (KeyValuesState.ContainsKey($"{Id}"))
            {
                KeyValuesState.Remove($"{Id}");
            }

            return Task.CompletedTask;
        }

        public string? GetUserState(long Id)
        {
            if (KeyValuesState.ContainsKey($"{Id}"))
            {
                return KeyValuesState[$"{Id}"];
            }
            return null;
        }

    }
}
