using System.IO;

namespace arts_core.Service
{
    public interface IFileService
    {
        Task<List<string>> StoreImageAsync(string storePath, ICollection<IFormFile> files);
        List<string> GetFilesName(string folderPath);
    }
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileService> _logger;
        public FileService(IWebHostEnvironment env, ILogger<FileService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public List<string> GetFilesName(string folderPath)
        {
            var filesName = new List<string>();
            var files = System.IO.Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                filesName.Add(Path.GetFileName(file));
            }
            return filesName;
        }

        public async Task<List<string>> StoreImageAsync(string storePath, ICollection<IFormFile> files)
        {
            try
            {
                var contentPath = _env.WebRootPath;
                var pathToImage = Path.Combine(contentPath, storePath);
                var fileNames = new List<string>();

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(pathToImage, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        fileNames.Add(uniqueFileName);
                    }
                }

                return fileNames;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something wrong in StoreImage");
                throw;
            }
        }
    }
}