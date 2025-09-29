using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace RealState.Controllers
{
    public class FileController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /File/PropertyImage?folder=properties&fileName=abc123.jpg
        [HttpGet]
        public IActionResult PropertyImage(string folder, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(fileName))
                return NotFound();

            // تحقق من الامتدادات المسموحة (لأغراض أمنية)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                return BadRequest("Invalid file type.");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Files", folder);
            var filePath = Path.Combine(folderPath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            // احصل على نوع المحتوى (MIME type)
            var contentType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType);
        }
    }
}