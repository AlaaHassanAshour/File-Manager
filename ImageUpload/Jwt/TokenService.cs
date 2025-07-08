using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text; 
using Microsoft.IdentityModel.Tokens; 

namespace ImageUpload.Jwt;
 
public class TokenService :  ITokenService
{
    private readonly IConfiguration _configuration; 

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<(string, string)> GenerateAccessTokenAsync(ClaimsPrincipal principal, int lifetime)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

        var tokenHandler = new JwtSecurityTokenHandler();
        var now = DateTime.Now;

        var claims = principal.Claims.ToList();

        var jwtId = Ulid.NewUlid().ToString();
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, jwtId));

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _configuration["Jwt:Issuer"]!.EndsWith('/')? _configuration["Jwt:Issuer"]!: _configuration["Jwt:Issuer"]!+"/",
            Expires = now.AddSeconds(lifetime),
            NotBefore = now,
            IssuedAt = now,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Task.FromResult((jwtId, tokenHandler.WriteToken(token)));
    }

    public string GenerateRefreshToken() => Ulid.NewUlid().ToString();
}
