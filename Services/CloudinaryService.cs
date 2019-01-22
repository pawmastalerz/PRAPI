using System.IO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PRAPI.Helpers;

namespace PRAPI.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly IOptions<CloudinarySettings> cloudinarySettings;
        private Cloudinary cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> cloudinarySettings)
        {
            this.cloudinarySettings = cloudinarySettings;

            Account acc = new Account(
                this.cloudinarySettings.Value.CloudName,
                this.cloudinarySettings.Value.ApiKey,
                this.cloudinarySettings.Value.ApiSecret
            );

            this.cloudinary = new Cloudinary(acc);
        }

        public bool CheckFile(IFormFile file)
        {
            // Accept only .jpg files no bigger than 6 MB
            if
            (
                (file.ContentType.ToLower() != "image/jpg" &&
                file.ContentType.ToLower() != "image/pjpeg" &&
                file.ContentType.ToLower() != "image/jpeg") ||
                (Path.GetExtension(file.FileName).ToLower() != ".jpg" &&
                Path.GetExtension(file.FileName).ToLower() != ".jpeg") ||
                (file.Length > 6291456)
            )
                return false;

            return true;
        }

        public ImageUploadResult UploadFile(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                        .Height(1000)
                        .Width(1000)
                        .Crop("fill")
                    };

                    uploadResult = this.cloudinary.Upload(uploadParams);
                }
            }
            return uploadResult;
        }

        public bool DeleteFile(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            var result = this.cloudinary.Destroy(deletionParams);
            if (result.Result == "ok")
                return true;
            
            return false;
        }
    }
}