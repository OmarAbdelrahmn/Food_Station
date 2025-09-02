namespace TechSpire.APi.Contracts.Auth.RefreshToken;

public record RefreshTokenRequest
(
    string Token,
    string RefreshToken
);
