using ImageUpload.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Xml.Linq;

namespace ImageUpload.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        // Allowed image MIME types
        private static readonly string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private static readonly string[] permittedMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };

        public UploadController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadDto model)
        {
            if (model.file == null || model.file.Length == 0)
                return BadRequest("No file uploaded.");

            // Validate extension and content type
            var extension = Path.GetExtension(model.file.FileName).ToLowerInvariant();
            if (!permittedExtensions.Contains(extension) || !permittedMimeTypes.Contains(model.file.ContentType))
                return BadRequest("Invalid file type. Only JPG, PNG, and GIF files are allowed.");

            // Sanitize inputs
            model.customFileName = Path.GetFileNameWithoutExtension(model.customFileName); // Strip extension

            // Handle path starting with '/'
            if (!model.path.IsNullOrEmpty() && model.path!.StartsWith("/"))
            {
                model.path = model.path.Substring(1);
            }

            var userFolder = Path.Combine(_env.WebRootPath, model.path);
            if (!Directory.Exists(userFolder))
                return BadRequest("Please add a correct path.");

            var finalFilePath = Path.Combine(userFolder, $"{model.customFileName}{extension}");

            if (System.IO.File.Exists(finalFilePath))
                return BadRequest("Custom File Name is already exist");

            // Save file
            using (var stream = new FileStream(finalFilePath, FileMode.Create))
            {
                await model.file.CopyToAsync(stream);
            }

            var relativePath = $"/{model.path}/{model.customFileName}{extension}";

            return Ok(new
            {
                message = "Upload successful",
                filePath = relativePath
            });
        }

        [AllowAnonymous]
        [HttpGet("GetImage")]
        public IActionResult GetImageUrl(string imagePath)
        {
            if (!imagePath.IsNullOrEmpty() && imagePath!.StartsWith("/"))
            {
                imagePath = imagePath.Substring(1);
            }
            var imageFullPath = Path.Combine(_env.WebRootPath, imagePath);

            if (!System.IO.File.Exists(imageFullPath))
                return NotFound("Image not found");


            var imageFileStream = new FileStream(imageFullPath, FileMode.Open, FileAccess.Read);
            var contentType = GetContentType(imagePath); // e.g., "image/jpeg"

            return File(imageFileStream, contentType);
        }

        [HttpDelete("DeleteImage")]
        public IActionResult DeleteImage(string imagePath)
        {
            if (!imagePath.IsNullOrEmpty() && imagePath!.StartsWith("/"))
            {
                imagePath = imagePath.Substring(1);
            }
            var imageFullPath = Path.Combine(_env.WebRootPath, imagePath);

            if (!System.IO.File.Exists(imageFullPath))
                return NotFound("Image not found");

            try
            {
                System.IO.File.Delete(imageFullPath);
                return Ok(new { message = "Image deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to delete image: {ex.Message}" });
            }
        }

        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }

    }

}
