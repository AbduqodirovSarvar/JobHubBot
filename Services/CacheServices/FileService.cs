using JobHubBot.Interfaces.IDbInterfaces;

namespace JobHubBot.Services.CacheServices
{
    public class FileService : IFileService
    {
        private readonly string _folderPath;
        public FileService()
        {
            _folderPath = Directory.GetCurrentDirectory();
        }

        public async Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(_folderPath, "Files", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            return fileName;
        }

        public FileStream? GetFileStream(string fileName)
        {
            string filePath = Path.Combine(_folderPath, "Files", fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file '{fileName}' does not exist in the 'Files' directory.");
            }

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
    }
}
