namespace JobHubBot.Services.CacheServices
{
    public class SaveFile
    {
        public async Task<string> SaveToFolderAsync(IFormFile file)
        {
            string folderPath = Directory.GetCurrentDirectory();
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(folderPath, "..", "..", "Files", fileName);
            string fp = Path.GetFullPath(filePath);

            using (var stream = new FileStream(fp, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
    }
}
