// RealState.BLL.Common.Services.AttachmentService/AttachmentService.cs
using Microsoft.AspNetCore.Http;
using System.IO;

namespace RealState.BLL.Common.Services.AttachmentService
{
    public class AttachmentService : IAttachmentService
    {
        private readonly List<string> _allowedExtensions = new() { ".jpg", ".png", ".jpeg", ".gif" };
        private const int _maxFileSizeInBytes = 3 * 1024 * 1024; // 3 MB

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(fileExtension))
                throw new Exception("File type is not allowed. Only JPG, PNG, GIF are allowed.");

            if (file.Length > _maxFileSizeInBytes)
                throw new Exception("File size exceeds the maximum limit of 3 MB.");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Files", folderName);
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            // نُرجع الرابط الذي يمر عبر FileController
            return $"/File/PropertyImage?folder={folderName}&fileName={fileName}";
        }

        public bool DeleteFile(string folderName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folderName) || string.IsNullOrWhiteSpace(fileName))
                return false;

            // تحقق من الامتداد (لأغراض أمنية)
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(ext))
                return false;

            // منع Directory Traversal
            if (folderName.Contains("..") || fileName.Contains(".."))
                return false;

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", folderName, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }

            return false;
        }
    }
}