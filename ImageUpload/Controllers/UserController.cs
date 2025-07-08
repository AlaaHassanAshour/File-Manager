using ImageUpload.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageUpload.Controllers
{
    [Authorize]
    [EnableCors("AllowConfiguredOrigins")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    { 
        [HttpGet("user-info")]
        public ActionResult<UserInfo> GetUserInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.Identity?.Name;
            var email = User.FindFirstValue(ClaimTypes.Email);

            var userInfo = new UserInfo
            {
                Username =userName!,
                Name = userName!,
                Email = email!
            };

            return Ok(userInfo);
        }
    }
}  
 
