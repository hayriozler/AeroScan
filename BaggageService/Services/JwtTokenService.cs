using Domain.Aggregates.Users;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BaggageService.Services;

public sealed class JwtTokenService(IConfiguration configuration)
{
    private const int _expiryHours = 8;

    public (string Token, DateTime ExpiresAt) GenerateToken(User user)
    {
        var signingKey = configuration["Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Jwt:SigningKey is not configured.");

        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt   = DateTime.UtcNow.AddHours(_expiryHours);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,        user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new("display_name",                     user.DisplayName),
            new("company_code",                     user.CompanyCode),
            new("company_name",                     user.Company?.Name ?? string.Empty),
            new("company_type",                     user.Company?.Type.ToString() ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),
        };

        //foreach (var role in user.GetRoleNames())
        //    claims.Add(new Claim(ClaimTypes.Role, role));

        foreach (var role in user.GetRoleNames())
        {
            claims.Add(new Claim("role", role));
        }

        var token = new JwtSecurityToken(
            issuer:            configuration["Jwt:Issuer"]   ?? "aeroscan",
            audience:          configuration["Jwt:Audience"] ?? "aeroscan-clients",
            claims:            claims,
            notBefore:         DateTime.UtcNow,
            expires:           expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
