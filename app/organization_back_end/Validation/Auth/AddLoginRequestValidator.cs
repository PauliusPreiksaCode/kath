using FluentValidation;
using organization_back_end.RequestDtos.Auth;

namespace organization_back_end.Validation.Auth;

public class AddLoginRequestValidator : AbstractValidator<LoginUserRequest>
{
    public AddLoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}