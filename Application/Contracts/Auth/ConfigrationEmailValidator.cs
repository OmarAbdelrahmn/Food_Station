using FluentValidation;

namespace Application.Contracts.Auth;

public class ConfigrationEmailValidator : AbstractValidator<ConfigrationEmailRequest>
{

    public ConfigrationEmailValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty();
    }
}

