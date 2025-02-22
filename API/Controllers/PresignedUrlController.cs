using BusinessLogicLayer.Models.General;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresignedUrlController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySettings _cloudinarySettings;

        public PresignedUrlController(IOptions<CloudinarySettings> cloudinarySettings)
        {
            _cloudinarySettings = cloudinarySettings.Value;

            var account = new Account(
                _cloudinarySettings.CloudName,
                _cloudinarySettings.ApiKey,
                _cloudinarySettings.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        [HttpGet("generate")]
        public IActionResult GeneratePresignedUrl()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 600;

            var signature = _cloudinary.Api.SignParameters(new System.Collections.Generic.SortedDictionary<string, object>
        {
            { "timestamp", timestamp },
            { "upload_preset", _cloudinarySettings.UploadPreset }
        });

            var presignedUrl = new
            {
                Url = $"https://api.cloudinary.com/v1_1/{_cloudinarySettings.CloudName}/image/upload",
                Fields = new
                {
                    api_key = _cloudinarySettings.ApiKey,
                    timestamp = timestamp,
                    upload_preset = _cloudinarySettings.UploadPreset,
                    signature = signature
                }
            };

            return Ok(presignedUrl);
        }

    }
}
