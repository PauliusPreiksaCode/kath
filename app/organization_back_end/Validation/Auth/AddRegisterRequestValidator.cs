using FluentValidation;
using organization_back_end.RequestDtos.Auth;

namespace organization_back_end.Validation.Auth;

public class AddRegisterRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public AddRegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(4).WithMessage("Username should be at least 4 characters long");

        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");
        
        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Surname is required");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
    }
}