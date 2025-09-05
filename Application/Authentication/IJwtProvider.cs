using Domain.Entities;

namespace Application.Authentication;

public interface IJwtProvider
{
    (string Token, int Expiry) GenerateToken(ApplicataionUser user, IEnumerable<string> Roles);

    string? ValidateToken(string token);
}
