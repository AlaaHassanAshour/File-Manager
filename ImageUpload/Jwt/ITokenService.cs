using System.Security.Claims; 

namespace ImageUpload.Jwt;

public interface ITokenService
{
    Task<(string, string)> GenerateAccessTokenAsync(ClaimsPrincipal principal, int lifetime);
    string GenerateRefreshToken();
}
