using Microsoft.Extensions.Logging;

namespace FileProviderWebServer.Services
{
    public class FileProviderService
    {
        private readonly ILogger<FileProviderService> _logger;
        private string _fileContent;

        public string FileContent
        {
            get => _fileContent;
            set
            {
                _fileContent = value;
                _logger.LogInformation($"new file content has been uploaded, content hash code: {_fileContent.GetHashCode()}");
            }
        }

        public FileProviderService(ILogger<FileProviderService> logger)
        {
            _logger = logger;
        }
    }
}
