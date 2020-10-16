using FileProviderWebServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileProviderWebServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileProviderController : Controller
    {
        private readonly FileProviderService _fileProviderService;

        public FileProviderController(FileProviderService fileProviderService)
        {
            _fileProviderService = fileProviderService;
        }

        [HttpGet]
        [Route("index")]
        public IActionResult Index()
        {
            ViewBag.FileContent = _fileProviderService.FileContent;
            return View();
        }

        [HttpGet]
        [Route("fileContentWithLinks")]
        public IActionResult FileContentWithLinks()
        {
            ViewBag.FileContent = _fileProviderService.FileContent;
            return View();
        }

        [HttpGet]
        public IActionResult FileContentUpload()
        {
            ViewBag.FileContent = _fileProviderService.FileContent;
            return View();
        }

        [HttpGet]
        [Route("fileContent")]
        public ContentResult FileContent()
        {
            return Content(_fileProviderService.FileContent);
        }

        [HttpPost]
        public IActionResult Post([FromForm] string fileContent)
        {
            if (string.IsNullOrEmpty(fileContent))
                return View("FileContentUpload");

            _fileProviderService.FileContent = fileContent;

            ViewBag.FileContent = _fileProviderService.FileContent;
            return View("FileContentWithLinks");
        }
    }
}
