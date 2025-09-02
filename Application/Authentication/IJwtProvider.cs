namespace SurvayBasket.Infrastructure.Authentication;

public interface IJwtProvider
{
    (string Token, int Expiry) GenerateToken(ApplicataionUser user, IEnumerable<string> Roles, IEnumerable<string> Permission);

    string? ValidateToken(string token);
}
