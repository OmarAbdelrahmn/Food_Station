namespace Application.Contracts.Auth;
public record ConfigrationEmailRequest
(
    string UserId,
    string Code
    );