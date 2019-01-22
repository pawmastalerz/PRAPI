using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace PRAPI.Services
{
    public interface ICloudinaryService
    {
        bool CheckFile(IFormFile file);
        ImageUploadResult UploadFile(IFormFile file);
        bool DeleteFile(string publicId);
    }
}