using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
namespace ImageUpload.Controllers
{
    [Authorize]
    [EnableCors("AllowConfiguredOrigins")]
    [ApiController]
    [Route("api/[controller]")]
    public class FoldersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public FoldersController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

     
        // CREATE - create new folder
        [HttpPost("create")]
        public IActionResult Create(string name, string? path="")
        {
            if (name.Contains('/'))
            {
                return BadRequest(new { message = "Please remove / character from name" });
            }
            if (!path.IsNullOrEmpty() && path!.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            
            var folderPath = Path.Combine(_env.WebRootPath, path!);
            //var folderPath = Path.Combine(_configuration["FoldersPath"]!, path!);
           
            if (!Directory.Exists(folderPath))
                return BadRequest(new { message = "The path was not found" });

             folderPath = Path.Combine(_env.WebRootPath, path!, name);

            if (Directory.Exists(folderPath))
                return Conflict( new
                {
                    message = "Folder already exists.",
                });

            Directory.CreateDirectory(folderPath);

            return Ok(new { message = "Folder was created successfully" });
        }
         

        // READ - get single folder info (check if exists)
        [HttpGet("browse")]
        public IActionResult Get(string? path ="")
        {
           
            var physicalPath = string.Empty;
            if (path!.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            //physicalPath = Path.Combine(_configuration["FoldersPath"]!, path);
            physicalPath = Path.Combine(_env.WebRootPath, path);
            try
                {
                    var provider = new PhysicalFileProvider(physicalPath);
                    // Get contents of the directory (not recursive)
                    var contents = provider.GetDirectoryContents(string.Empty);


                    if (!Directory.Exists(physicalPath))
                        return BadRequest(new { message = "The path was not found" });

                    return Ok(contents);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = string.Format("This path `{0}` was not found", ex.Message) });
                }

           
        }

        // UPDATE - rename folder
        [HttpPut("rename")]
        public IActionResult Rename(string name, string? path = "")
        {
            if (name.Contains('/'))
            {
                return BadRequest(new { message = "Please remove / character from name" });
            }
            if (!path.IsNullOrEmpty() && path!.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            var oldPath = Path.Combine(_env.WebRootPath, path!);


            // Find position of the last slash
            int lastSlashIndex = path!.LastIndexOf('/');

            // Remove everything after the last slash (including the slash)
            var newPath = lastSlashIndex >= 0 ?  Path.Combine(_env.WebRootPath, path.Substring(0, lastSlashIndex), name)
                : Path.Combine(_env.WebRootPath, name);

            if (oldPath == newPath)
                return Ok();

            if (!Directory.Exists(oldPath))
                return NotFound("Original folder does not exist.");

            if (Directory.Exists(newPath))
                return Conflict("Target folder name already exists.");

            Directory.Move(oldPath, newPath);

            return Ok(new { message = "Folder was renamed successfully", path = newPath });
        }

        // DELETE - delete folder
        [HttpDelete("delete")]
        public IActionResult Delete(string path)
        {
            if (!path.IsNullOrEmpty() && path!.StartsWith("/"))
            {
                path = path.Substring(1);
                if (path.IsNullOrEmpty())
                {
                    return BadRequest(new { message = "Pleaseb add a correct path" });
                }
            }
            var folderPath = Path.Combine(_env.WebRootPath, path);

            if (!Directory.Exists(folderPath))
                return BadRequest(new { message = "The path was not found" });

            Directory.Delete(folderPath, recursive: true);

            return Ok(new { message = "Folder was deleted successfully"});
        }

        // Rename image file
        [HttpPut("renameFile")]
        public IActionResult RenameFile(string name, string? path = "")
        {
            if (name.Contains('/'))
            {
                return BadRequest(new { message = "Please remove / character from name" });
            }

            if (!path.IsNullOrEmpty() && path!.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            var oldPath = Path.Combine(_env.WebRootPath, path!);
            
            if (!System.IO.File.Exists(oldPath))
                return NotFound("Original file does not exist.");

            // Get the directory path and create new path
            var directory = Path.GetDirectoryName(oldPath);
            var oldExtension = Path.GetExtension(oldPath);
            var newPath = Path.Combine(directory!, name);

            // If the new name doesn't include the extension, add it
            if (!name.EndsWith(oldExtension, StringComparison.OrdinalIgnoreCase))
            {
                newPath += oldExtension;
            }

            if (oldPath.Equals(newPath, StringComparison.OrdinalIgnoreCase))
                return Ok();

            if (System.IO.File.Exists(newPath))
                return Conflict("Target file name already exists.");

            System.IO.File.Move(oldPath, newPath);

            return Ok(new { message = "File was renamed successfully", path = newPath });
        }
    }

}
