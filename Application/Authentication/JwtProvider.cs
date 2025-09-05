﻿using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SurvayBasket.Infrastructure.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Application.Authentication;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions options = options.Value;

    public (string Token, int Expiry) GenerateToken(ApplicataionUser user, IEnumerable<string> Roles)
    {
        Claim[] claims = [
            new (System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Id),
            new (System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, user.Email!),
            new (System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.GivenName, user.FirstName),
            new (System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.FamilyName, user.LastName),
            new (System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (nameof(Roles),JsonSerializer.Serialize(Roles),System.IdentityModel.Tokens.Jwt.JsonClaimValueTypes.JsonArray)
            ];

        var SymmetricSecuritykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));

        var signingCredentials = new SigningCredentials(SymmetricSecuritykey, SecurityAlgorithms.HmacSha256);


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

            return jwtToken.Claims.First(claim => claim.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub).Value;
        }
        catch
        {
            return null;

        }
    }
}
