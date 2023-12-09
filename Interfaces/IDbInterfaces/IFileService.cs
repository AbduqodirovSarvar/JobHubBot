namespace JobHubBot.Interfaces.IDbInterfaces
{
    public interface IFileService
    {
        Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken);

        FileStream? GetFileStream(string fileName);
    }
}
