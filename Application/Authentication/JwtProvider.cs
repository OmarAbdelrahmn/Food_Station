using System.Text.Json;

namespace SurvayBasket.Infrastructure.Authentication;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions options = options.Value;

    public (string Token, int Expiry) GenerateToken(ApplicataionUser user, IEnumerable<string> Roles, IEnumerable<string> Permission)
    {
        Claim[] claims = [
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (JwtRegisteredClaimNames.Email, user.Email!),
            new (JwtRegisteredClaimNames.GivenName, user.FirstName),
            new (JwtRegisteredClaimNames.FamilyName, user.LastName),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (nameof(Roles),JsonSerializer.Serialize(Roles),JsonClaimValueTypes.JsonArray),
            new (nameof(Permission),JsonSerializer.Serialize(Permission),JsonClaimValueTypes.JsonArray)
            ];

        var SymmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));

        var signingCredentials = new SigningCredentials(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);


        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(options.ExpiryIn),
            signingCredentials: signingCredentials
        );

        return (Token: new JwtSecurityTokenHandler().WriteToken(token), Expiry: options.ExpiryIn);
    }

    public string? ValidateToken(string token)
    {
        var tokenhandler = new JwtSecurityTokenHandler();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));

        try
        {
            tokenhandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = key
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            return jwtToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
        }
        catch
        {
            return null;

        }
    }
}
