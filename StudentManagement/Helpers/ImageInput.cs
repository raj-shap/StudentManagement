using StudentManagement.Models;
using System.Linq;

namespace StudentManagement.Helpers
{
    public static class ImageInput
    {
        public static async Task<string> ImageInputHelper(IFormFile photo)
        {
            if(photo == null || photo.Length == 0)
            {
                throw new ArgumentException("No File Selected");
            }
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(photo.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                    throw new Exception("Invalid file type. Only image file are allowed");
            }
            var fileName = Path.GetFileNameWithoutExtension(photo.FileName);
            var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", uniqueFileName);

            var directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }
            }
            catch (IOException ex)
            {
                throw new Exception("An error occurred while uploading the file.", ex);
            }
            return $"images/{uniqueFileName}";
        }
    }
}
