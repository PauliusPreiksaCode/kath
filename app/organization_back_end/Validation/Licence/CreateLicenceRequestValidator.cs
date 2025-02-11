using FluentValidation;
using organization_back_end.Enums;
using organization_back_end.RequestDtos.Licences;

namespace organization_back_end.Validation.Licence;

public class CreateLicenceRequestValidator : AbstractValidator<CreateLicenceRequest>
{
    public CreateLicenceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price is required");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Type is required");
        
        RuleFor(x => x.Duration)
            .NotEmpty().WithMessage("Duration is required");
    }   
}