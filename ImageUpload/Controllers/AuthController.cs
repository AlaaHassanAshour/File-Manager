using ImageUpload.Dtos;
using ImageUpload.Jwt;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc; 

namespace ImageUpload.Controllers
{
    [EnableCors("AllowConfiguredOrigins")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;  
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserClaimsPrincipalFactory<IdentityUser> _claimsFactory;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager,
                                    ITokenService tokenService, IUserClaimsPrincipalFactory<IdentityUser> claimsFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _tokenService = tokenService;
            _claimsFactory = claimsFactory;
        }
    
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);
    
            if (result.Succeeded)
                return Ok("User registered");
    
            return BadRequest(result.Errors);
        } 

        [HttpPost("login")]
        public async Task<LoginResult<UserInfo>> Login(LoginWithUsernameInput input)
        {
            var user = await _userManager.FindByNameAsync(input.Username);
            var lifetime = _configuration.GetValue<int>("Jwt:ExpiresInMinutes");

            if (user is null)
            { 
                return new LoginResult<UserInfo>
                {
                    ErrorMessage = "Incorrect UserName Or Password",
                    AccessToken = "",
                    ExpiresIn = lifetime,
                    RefreshToken = null,
                    UserInfo = null
                };
            }

            var loginResult = await _signInManager.CheckPasswordSignInAsync(user, input.Password, false);

            if (!loginResult.Succeeded)
            { 
                return new LoginResult<UserInfo>
                {
                    ErrorMessage = "Incorrect UserName Or Password",
                    AccessToken = "",
                    ExpiresIn = lifetime,
                    RefreshToken = null,
                    UserInfo = null
                };
            }

            var principal = await _claimsFactory.CreateAsync(user);

            var (_, token) = await _tokenService.GenerateAccessTokenAsync(principal, lifetime);

            return new LoginResult<UserInfo>
            {
                AccessToken = token,
                ExpiresIn = lifetime,
                RefreshToken = null,
                UserInfo = new UserInfo
                {
                    Username = user.UserName,
                    Name = user.NormalizedUserName,
                    Email = user.Email
                }
            };
        }

    }
}  
 
