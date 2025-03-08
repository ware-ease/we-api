using BusinessLogicLayer.IServices;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _uploadPreset;
        private readonly string _apiKey;
        private readonly string _cloudName;

        public CloudinaryService(IConfiguration configuration)
        {
            _cloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME") ?? configuration["Cloudinary:CloudName"];
            _apiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY") ?? configuration["Cloudinary:ApiKey"];
            var apiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET") ?? configuration["Cloudinary:ApiSecret"];
            _uploadPreset = Environment.GetEnvironmentVariable("CLOUDINARY_UPLOAD_PRESET") ?? configuration["Cloudinary:UploadPreset"];

            var account = new Account(_cloudName, _apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<object> GeneratePresignedUrl()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 600;

            var signature = _cloudinary.Api.SignParameters(new SortedDictionary<string, object>
            {
                { "timestamp", timestamp },
                { "upload_preset", _uploadPreset }
            });

            return new
            {
                Url = $"https://api.cloudinary.com/v1_1/{_cloudName}/image/upload",
                Fields = new
                {
                    api_key = _apiKey,
                    timestamp = timestamp,
                    upload_preset = _uploadPreset,
                    signature = signature
                }
            };
        }
    }
}
