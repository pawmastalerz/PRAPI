using Microsoft.AspNetCore.Http;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace PRAPI.Services
{
    public interface ICloudinaryService
    {
        bool CheckFile(IFormFile file);
        ImageUploadResult UploadFile(IFormFile file);
        bool DeleteFile(string publicId);
    }
}