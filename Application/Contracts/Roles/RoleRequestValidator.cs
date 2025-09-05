using FluentValidation;

namespace Application.Contracts.Roles;

public class RoleRequestValidator : AbstractValidator<RoleRequest>
{
    public RoleRequestValidator()
    {
        RuleFor(i => i.Name)
            .NotEmpty()
            .Length(3, 256);

    }
}
