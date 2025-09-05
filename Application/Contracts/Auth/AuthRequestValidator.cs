using FluentValidation;

namespace Application.Contracts.Auth;

public class RefreshTokenRequestValidator : AbstractValidator<AuthRequest>
{

    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .Length(8, 50);

    }
}
