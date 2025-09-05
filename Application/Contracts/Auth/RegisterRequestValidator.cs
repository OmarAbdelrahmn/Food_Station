using Application.Abstraction.Consts;
using FluentValidation;

namespace Application.Contracts.Auth;



public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{

    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .Length(8,16)
            .WithMessage("Password must be between 8 and 16 characters")
            .Matches(RegexPatterns.Password)
            .WithMessage("Password should contains Lowercase,Uppercase,Number and Special character ");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("LastName is required")
            .Length(3, 100);


        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("LastName is required")
            .Length(3, 100);
    }
}

