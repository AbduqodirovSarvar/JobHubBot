namespace JobHubBot.Interfaces.IDbInterfaces
{
    public interface ICacheDbService
    {
        Task DeleteAsync(string key);
        Task<T?> GetObjectAsync<T>(string key) where T : class;
        Task SetObjectAsync<T>(string key, T obj) where T : class;
    }
}
