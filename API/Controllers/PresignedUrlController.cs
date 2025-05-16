using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Models.General;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    [Route("api/cloudinary")]
    [ApiController]
    public class PresignedUrlController : ControllerBase
    {
        //private readonly Cloudinary _cloudinary;
        //private readonly CloudinarySettings _cloudinarySettings;
        private readonly ICloudinaryService _cloudinaryService;


        public PresignedUrlController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }
        [Authorize]
        [HttpGet("generate")]
        public async Task<IActionResult> GeneratePresignedUrl()
        {
            var presignedUrl = await _cloudinaryService.GeneratePresignedUrl();
            return Ok(presignedUrl);
        }

        /*[HttpGet("generate")]
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
        }*/

    }
}
